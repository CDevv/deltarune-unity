/*
(Attached to a TextboxManager)
A Textbox Manager manages the Textbox gameObject (enabling or disabling it), the text, the voice, and the image it has to display.
StartDialogue(dialogEvent) takes a DialogueEvent class attribute, which contains an array of DialogTextbox, another class that has a lot of useful properties (refer to their script to learn more about them).

When called, it will load the DialogTextBox(es) in a queue, ready to be read and displayed by DisplayNextSentence();
DisplayNextSentence will :
- Scroll the text with varying time delays depending on the values of ShortDelay and LongDelay.
- Speak with the voice of the Character (by reading everything before the "_").
- Display the face stored in Character.
- Use a custom size/position when specified (useful for cutscenes).
- Call EndDialogue() when the Queue of DialoxTextBox is over.

All in all, this is a pretty versatile script.
*/
using System.Collections;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class DialogueManager : MonoBehaviour
{
	public Text textboxText;
	public GameObject goTextbox;
	public Image textboxImage;
	[HideInInspector]
	public DialogTextBox Textbox;
	[HideInInspector]
	public string fullText;
	Queue<DialogTextBox> Textboxes;
	Dictionary<string, string> Faces = new Dictionary<string, string>();
	AudioClip voice;
	AudioSource audioClip;
	Coroutine co;
	RectTransform rt = new RectTransform();
	bool typing;

	public GameObject heart;
	bool optionsAvailable = false;
	DialogOptions[] dialogOptions = new DialogOptions[0];
	int optionChosen = -1; //-1 for no option is chosen

	private Dictionary<string, DialogEvent> textLibrary = new Dictionary<string, DialogEvent>();

	void Start()
	{
		audioClip = GetComponent<AudioSource>();
		Textboxes = new Queue<DialogTextBox>();
		rt = textboxImage.GetComponent<RectTransform>();
		string json = Resources.Load<TextAsset>("Json/faces").text;
		Faces = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

		json = Resources.Load<TextAsset>("Json/text_en").text;
		textLibrary = JsonConvert.DeserializeObject<Dictionary<string, DialogEvent>>(json);

		heart = goTextbox.transform.Find("Heart").gameObject;
	}

	void Update()
	{
		float inputHorizontal = Input.GetAxisRaw("Horizontal");
		float inputVertical = Input.GetAxisRaw("Vertical");

		if (Input.GetButtonDown("Confirm"))
		{
			if (!optionsAvailable)
			{
				if (typing)
				{
					if (co != null) { StopCoroutine(co); }
					typing = false;
					textboxText.text = fullText;
				}
				else
				{
					DisplayNextSentence();
				}
			}
			else
			{
				if (optionChosen != -1)
				{
					StartDialogue(textLibrary[dialogOptions[optionChosen - 1].DialogID]);
				}
			}
		}

		//up, down, left, right
		if (inputVertical > 0)
		{
			if (dialogOptions.Length > 2)
			{
				ChangeOption(3);
			}
		}
		if (inputVertical < 0)
		{
			if (dialogOptions.Length > 3)
			{
				ChangeOption(4);
			}
		}
		if (inputHorizontal > 0)
		{
			if (dialogOptions.Length > 1)
			{
				ChangeOption(2);
			}
		}
		if (inputHorizontal < 0)
		{
			if (dialogOptions.Length > 0)
			{
				ChangeOption(1);
			}
		}
	}

	public void StartDialogue(DialogEvent dialogEvent)
	{
		textboxImage.gameObject.SetActive(true);
		for (int i = 0; i < dialogOptions.Length; i++)
		{
			string optionName = $"TextboxOption{i+1}";
			var option = goTextbox.transform.Find(optionName).gameObject;
			option.GetComponent<Text>().color = new Color(255, 255, 255, 255);
			option.SetActive(false);
		}
		dialogOptions = new DialogOptions[0];
		heart.SetActive(false);
		RectTransform heartRect = heart.GetComponent<RectTransform>();
		heartRect.anchoredPosition = new Vector2(0, 0);

		PlayerController.inMenu = true;
		MenuTop.enableMenu = false;
		Textboxes.Clear();
		goTextbox.SetActive(true);

		foreach (DialogTextBox Textbox in dialogEvent.TextBoxes)
		{
			//Debug.Log(Textbox.Character);
			Textboxes.Enqueue(Textbox);
		}
		DisplayNextSentence();
	}

	public void DisplayNextSentence()
	{
		string voiceStr = "";

		if (Textboxes.Count == 0)
		{
			StopAllCoroutines();
			EndDialogue();
			return;
		}
		DialogTextBox Textbox = Textboxes.Dequeue();

		// VOICE OF THE TEXTBOX
		if (Textbox.Character != null)
		{
			string[] split = Textbox.Character.Split('_');  // Formatting :)
			voiceStr = split[0];
		}
		voice = Resources.Load<AudioClip>("Audio/Voices/Voice_" + voiceStr);
		audioClip.clip = voice;

		// FACE SPRITE OF THE TEXTBOX
		if (Textbox.Character == null)
		{
			textboxImage.color = new Color(255, 255, 255, 0);
		}
		else if (Textbox.Character != "Narrator" || Textbox.Character != "options")
		{
			textboxImage.color = new Color(255, 255, 255, 255);
			textboxImage.sprite = Resources.Load<Sprite>("Sprites/Faces/" + Faces[Textbox.Character]);
			rt.sizeDelta = new Vector2(textboxImage.sprite.rect.width, textboxImage.sprite.rect.height);
		}
		else
		{
			textboxImage.color = new Color(255, 255, 255, 0);
		}

		// POSITION OF THE TEXTBOX RECT
		if (Textbox.Pos != new Vector2(0, 0))
		{
			textboxText.rectTransform.anchoredPosition = Textbox.Pos;  // Use custom pos when set
		}
		else if (Textbox.Character != null)
		{
			textboxText.rectTransform.anchoredPosition = new Vector2(-42, -6); // Default pos for character textboxes
		}
		else
		{
			textboxText.rectTransform.anchoredPosition = new Vector2(5, 0); // Default pos for floating text
		}

		// SIZE OF THE TEXTBOX RECT
		if (Textbox.Size != new Vector2(0, 0))
		{
			textboxText.rectTransform.sizeDelta = Textbox.Size;  // Use custom size when set
		}
		else if (Textbox.Character == "Narrator" || Textbox.Character == null)
		{
			textboxText.rectTransform.sizeDelta = new Vector2(505, 125);  // Use larger textarea when narrating
		}
		else
		{
			textboxText.rectTransform.sizeDelta = new Vector2(400, 125);   // Default size
		}

		// TEXT FORMATTING
		fullText = WordWrap(Textbox.Text, -2 + (int)textboxText.rectTransform.sizeDelta.x / 16);



		if (co != null)
			StopCoroutine(co);

		co = StartCoroutine(TypeSentence(fullText, Textbox.Character, Textbox.Options, Textbox.ShortDelay, Textbox.LongDelay));
	}

	IEnumerator TypeSentence(string sentence, string character, DialogOptions[] options, float shortDelay = .033f, float longDelay = .066f) // The typing and speaking engine
	{
		typing = true;

		if (shortDelay == 0) { shortDelay = .033f; }
		if (longDelay == 0) { longDelay = .066f; }

		StringBuilder textboxTextSB = new StringBuilder();

		if (options != null)
		{
			optionsAvailable = true;
			dialogOptions = options;

			audioClip.Play();
			textboxImage.sprite = null;
			textboxImage.gameObject.SetActive(false);
			heart.SetActive(true);
			for (int i = 0; i < options.Length; i++)
			{
				string optionName = $"TextboxOption{i + 1}";
				var optionText = goTextbox.transform.Find(optionName).gameObject; //Find the option object
				optionText.GetComponent<Text>().text = options[i].Title; //Set option text
				Debug.Log(options[i]);
				optionText.SetActive(true); //Make it visible
				textboxText.text = textboxTextSB.ToString();
			}
		}
		else
		{
			optionsAvailable = false;
			foreach (char letter in sentence)
			{
				textboxTextSB.Append(letter);                // Appends letter for the typing effect
				textboxText.text = textboxTextSB.ToString(); // Converts to string to be displayed by the textbox

				char[] array = { ',', '.', '?', '!' };

				if (array.Contains(letter))     // No sound, longer pause
				{
					yield return new WaitForSeconds(longDelay);

				}
				else if (letter != ' ')
				{       // Normal pause, no sound
					audioClip.Play();
				}

				yield return new WaitForSeconds(shortDelay);
			}
		}
		typing = false;
	}

	public void EndDialogue()
	{
		goTextbox.SetActive(false);
		textboxText.text = "";
		textboxImage.gameObject.SetActive(false);
		for (int i = 0; i < dialogOptions.Length; i++)
		{
			string optionName = $"TextboxOption{i+1}";
			var option = goTextbox.transform.Find(optionName).gameObject;
			option.GetComponent<Text>().color = new Color(255, 255, 255, 255);
			option.SetActive(false);
		}
		dialogOptions = new DialogOptions[0];
		PlayerController.inMenu = false;
		MenuTop.enableMenu = true;
	}

	private static string WordWrap(string text, int maxLineLength)
	{
		var list = new List<string>();

		int currentIndex;
		var lastWrap = 0;
		var whitespace = new[] { ' ', '\r', '\n', '\t' };
		do
		{
			currentIndex =
				lastWrap +
				maxLineLength > text.Length
					? text.Length
					: (text.LastIndexOfAny(new[] { ' ', ',', '.', '?', '!', ':', ';', '-', '\n', '\r', '\t' },
							Math.Min(text.Length - 1, lastWrap + maxLineLength)) + 1);
			if (currentIndex <= lastWrap)
				currentIndex = Math.Min(lastWrap + maxLineLength, text.Length);
			list.Add(text.Substring(lastWrap, currentIndex - lastWrap).Trim(whitespace));
			lastWrap = currentIndex;
		} while (currentIndex < text.Length);

		string str = "";

		foreach (string line in list)
			str += line + "\n";

		return str.TrimEnd('\n');
	}

	private void ChangeOption(int n)
	{
		string optionName = "";
		if (optionChosen != -1)
		{
			optionName = $"TextboxOption{optionChosen}";
			var option = goTextbox.transform.Find(optionName).gameObject;
			option.GetComponent<Text>().color = new Color(255, 255, 255, 255);
		}
		optionChosen = n;
		optionName = $"TextboxOption{optionChosen}";
		var optionText = goTextbox.transform.Find(optionName).gameObject;
		optionText.GetComponent<Text>().color = new Color(255, 255, 0, 255);

		RectTransform heartRect = heart.GetComponent<RectTransform>();
		RectTransform optionRect = optionText.GetComponent<RectTransform>();
		Vector2 vector2 = new Vector2(optionRect.anchoredPosition.x + optionRect.rect.width / 2, optionRect.anchoredPosition.y);
		heartRect.anchoredPosition = vector2;
	}
}