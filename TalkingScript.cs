using UnityEngine;

public class TalkingScript : MonoBehaviour
{
	private const float LongestTime = 100f;

	private const float LongTime = 5f;

	private const float MediumTime = 3f;

	private const float ShortTime = 2f;

	public StudentScript S;

	public WeaponScript StuckBoxCutter;

	public bool NegativeResponse;

	public bool RejectGossip;

	public bool Eighties;

	public bool FadeIn;

	public bool Follow;

	public bool Grudge;

	public bool Refuse;

	public bool Fake;

	public string IdleAnim = "";

	public float Timer;

	public int ClubBonus;

	public string RejectGossipLine;

	public MusicTest AudioData;

	public GameObject VoiceClip;

	private void Start()
	{
		Eighties = GameGlobals.Eighties;
		AudioData = S.StudentManager.AudioData;
	}

	private void Update()
	{
		if (!S.Talking)
		{
			return;
		}
		S.BoobsResized = false;
		if (S.Sleuthing && S.Club == ClubType.Photography)
		{
			ClubBonus = 5;
		}
		else
		{
			ClubBonus = 0;
		}
		if (S.StudentManager.EmptyDemon)
		{
			ClubBonus = (int)S.Club * -1;
		}
		if (S.Interaction == StudentInteractionType.Idle)
		{
			if (!Fake)
			{
				if (S.Yandere.TargetStudent != null && S.StudentID == 10 && S.FollowTarget != null && S.FollowTarget.Routine && !S.FollowTarget.Distracted)
				{
					S.FollowTarget.Pathfinding.canSearch = false;
					S.FollowTarget.Pathfinding.canMove = false;
					S.FollowTarget.FocusOnYandere = true;
					S.FollowTarget.Routine = false;
				}
				if (S.Sleuthing)
				{
					IdleAnim = S.SleuthCalmAnim;
				}
				else if (S.Club == ClubType.Art && S.DialogueWheel.ClubLeader && S.Paintbrush.activeInHierarchy && !S.StudentManager.Eighties)
				{
					IdleAnim = "paintingIdle_00";
				}
				else if (S.Club != ClubType.Bully)
				{
					IdleAnim = S.IdleAnim;
				}
				else if (S.StudentManager.Reputation.Reputation < 33.33333f || S.Persona == PersonaType.Coward)
				{
					if (S.CurrentAction == StudentActionType.Sunbathe && S.SunbathePhase > 2)
					{
						IdleAnim = S.OriginalIdleAnim;
					}
					else
					{
						IdleAnim = S.IdleAnim;
					}
				}
				else
				{
					IdleAnim = S.CuteAnim;
				}
				S.CharacterAnimation.CrossFade(IdleAnim);
			}
			else if (IdleAnim != "")
			{
				S.CharacterAnimation.CrossFade(IdleAnim);
			}
			if (S.TalkTimer == 0f)
			{
				if (!S.DialogueWheel.AppearanceWindow.Show && !S.StudentManager.TutorialActive && !S.StudentManager.KokonaTutorial)
				{
					S.DialogueWheel.Impatience.fillAmount += Time.deltaTime * 0.1f;
				}
				if (S.DialogueWheel.Impatience.fillAmount > 0.5f && !S.StudentManager.Police.FadeOut && !S.StudentManager.Police.Show && S.Subtitle.Timer == 0f)
				{
					if (S.StudentID == 41 && !S.StudentManager.Eighties)
					{
						S.Subtitle.UpdateLabel(SubtitleType.Impatience, 4, 5f);
					}
					else if (S.Pestered == 0)
					{
						S.Subtitle.UpdateLabel(SubtitleType.Impatience, 0, 5f);
					}
					else
					{
						S.Subtitle.UpdateLabel(SubtitleType.Impatience, 2, 5f);
					}
				}
				if (S.DialogueWheel.Impatience.fillAmount == 1f && S.DialogueWheel.Show)
				{
					if (S.StudentID == 41 && !S.StudentManager.Eighties)
					{
						S.Subtitle.UpdateLabel(SubtitleType.Impatience, 4, 5f);
					}
					else if (S.Club == ClubType.Delinquent)
					{
						S.Subtitle.UpdateLabel(SubtitleType.Dismissive, 3, 5f);
					}
					else if (S.Pestered == 0)
					{
						S.Subtitle.UpdateLabel(SubtitleType.Impatience, 1, 5f);
					}
					else
					{
						S.Subtitle.UpdateLabel(SubtitleType.Impatience, 3, 5f);
					}
					S.WaitTimer = 0f;
					S.Pestered += 5;
					S.DialogueWheel.Pestered = true;
					S.DialogueWheel.End();
				}
			}
		}
		else if (S.Interaction == StudentInteractionType.Forgiving)
		{
			if (S.TalkTimer == 3f)
			{
				if (S.Club != ClubType.Delinquent)
				{
					S.CharacterAnimation.CrossFade(S.Nod2Anim);
					S.RepRecovery = 5f;
					if (PlayerGlobals.PantiesEquipped == 6)
					{
						S.RepRecovery += 2.5f;
					}
					if (S.Yandere.Class.SocialBonus > 0)
					{
						S.RepRecovery += 2.5f;
					}
					S.PendingRep += S.RepRecovery;
					S.Reputation.PendingRep += S.RepRecovery;
					for (int i = 0; i < S.Outlines.Length; i++)
					{
						if (!(S.Outlines[i] != null))
						{
							continue;
						}
						if (!S.Rival)
						{
							if (S.StudentManager.StudentPhotographed[S.StudentID])
							{
								Debug.Log("They're not a rival, and they're photographed, so they should be green!");
								S.Outlines[i].color = new Color(0f, 1f, 0f);
							}
							else
							{
								S.Outlines[i].enabled = false;
							}
						}
						else
						{
							Debug.Log("She's a rival! She's going back to being red!");
							S.Outlines[i].color = new Color(1f, 0f, 0f);
						}
						S.Outlines[i].enabled = true;
					}
					S.Forgave = true;
					if (S.Witnessed == StudentWitnessType.Insanity || S.Witnessed == StudentWitnessType.WeaponAndBloodAndInsanity || S.Witnessed == StudentWitnessType.WeaponAndInsanity || S.Witnessed == StudentWitnessType.BloodAndInsanity)
					{
						S.Subtitle.UpdateLabel(SubtitleType.ForgivingInsanity, 0, 3f);
					}
					else if (S.Witnessed == StudentWitnessType.Accident)
					{
						S.Subtitle.UpdateLabel(SubtitleType.ForgivingAccident, 0, 5f);
					}
					else
					{
						S.Subtitle.UpdateLabel(SubtitleType.Forgiving, 0, 3f);
					}
				}
				else
				{
					S.Subtitle.UpdateLabel(SubtitleType.Dismissive, 0, 5f);
				}
			}
			else
			{
				if (Input.GetButtonDown(InputNames.Xbox_A))
				{
					S.TalkTimer = 0f;
				}
				if (S.CharacterAnimation[S.Nod2Anim].time >= S.CharacterAnimation[S.Nod2Anim].length)
				{
					S.CharacterAnimation.CrossFade(IdleAnim);
				}
				if (S.TalkTimer <= 0f)
				{
					S.IgnoreTimer = 5f;
					S.DialogueWheel.End();
				}
			}
			S.TalkTimer -= Time.deltaTime;
		}
		else if (S.Interaction == StudentInteractionType.ReceivingCompliment)
		{
			if (S.TalkTimer == 3f)
			{
				if (!ConversationGlobals.GetTopicDiscovered(20))
				{
					S.Yandere.NotificationManager.TopicName = "Socializing";
					S.Yandere.NotificationManager.DisplayNotification(NotificationType.Topic);
					ConversationGlobals.SetTopicDiscovered(20, value: true);
				}
				if (!S.StudentManager.GetTopicLearnedByStudent(20, S.StudentID))
				{
					S.Yandere.NotificationManager.TopicName = "Socializing";
					S.Yandere.NotificationManager.DisplayNotification(NotificationType.Opinion);
					S.StudentManager.SetTopicLearnedByStudent(20, S.StudentID, boolean: true);
				}
				if (S.Club != ClubType.Delinquent)
				{
					S.CharacterAnimation.CrossFade(S.LookDownAnim);
					int topicSelected = S.StudentManager.DialogueWheel.TopicInterface.TopicSelected;
					if (!S.StudentManager.GetTopicLearnedByStudent(topicSelected, S.StudentID))
					{
						S.Yandere.NotificationManager.TopicName = S.StudentManager.InterestManager.TopicNames[topicSelected];
						S.Yandere.NotificationManager.DisplayNotification(NotificationType.Opinion);
						S.StudentManager.SetTopicLearnedByStudent(topicSelected, S.StudentID, boolean: true);
					}
					if (S.DialogueWheel.TopicInterface.Success)
					{
						CalculateRepBonus();
						S.Subtitle.PersonaSubtitle.UpdateLabel(PersonaType.Nemesis, S.Reputation.Reputation, 5f);
						S.Reputation.PendingRep += 1f + (float)S.RepBonus;
						S.PendingRep += 1f + (float)S.RepBonus;
					}
					else
					{
						S.Subtitle.PersonaSubtitle.UpdateLabel(PersonaType.None, S.Reputation.Reputation, 5f);
						S.Reputation.PendingRep -= 1f;
						S.PendingRep -= 1f;
					}
					Debug.Log("Attempting to log the fact that the player has discussed topic #" + topicSelected + " with Student #" + S.StudentID);
					S.StudentManager.SetTopicDiscussedWithStudent(topicSelected, S.StudentID, boolean: true);
				}
				else
				{
					S.Subtitle.UpdateLabel(SubtitleType.Dismissive, 1, 3f);
				}
			}
			else if (Input.GetButtonDown(InputNames.Xbox_A))
			{
				S.TalkTimer = 0f;
			}
			S.TalkTimer -= Time.deltaTime;
			if (S.TalkTimer <= 0f)
			{
				S.DialogueWheel.PromptBar.ClearButtons();
				S.DialogueWheel.PromptBar.Label[0].text = "Confirm";
				S.DialogueWheel.PromptBar.Label[4].text = "Change Selection";
				S.DialogueWheel.PromptBar.UpdateButtons();
				S.DialogueWheel.PromptBar.Show = true;
				S.DialogueWheel.Social.gameObject.SetActive(value: true);
				S.Interaction = StudentInteractionType.Idle;
				if (S.DialogueWheel.TopicInterface.Positive)
				{
					S.DialogueWheel.Social.SpokePositive[S.StudentID] = true;
				}
				else
				{
					S.DialogueWheel.Social.SpokeNegative[S.StudentID] = true;
				}
				S.DialogueWheel.Social.DialogueLabel.text = S.DialogueWheel.Social.Dialogue[1];
				S.DialogueWheel.Social.UpdateButtons();
				S.Yandere.HUD.alpha = 0f;
				Time.timeScale = 0.0001f;
			}
		}
		else if (S.Interaction == StudentInteractionType.Gossiping)
		{
			if (S.TalkTimer == 3f)
			{
				if (S.Club != ClubType.Delinquent)
				{
					S.Gossiped = true;
					CheckForGossipSpecialCase();
					if (RejectGossip)
					{
						S.CharacterAnimation.CrossFade(S.GossipAnim);
						S.Subtitle.CustomText = RejectGossipLine;
						S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
						S.Reputation.PendingRep -= 1f;
						S.PendingRep -= 1f;
					}
					else
					{
						if (S.DialogueWheel.TopicInterface.Success)
						{
							S.CharacterAnimation.CrossFade(S.GossipAnim);
							S.Subtitle.CustomText = "Ugh, I can't get along with people like that...";
							S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
							S.GossipBonus = 0;
							if (PlayerGlobals.PantiesEquipped == 9)
							{
								S.GossipBonus++;
							}
							if (S.Yandere.Class.SocialBonus > 0)
							{
								S.GossipBonus++;
							}
							if (S.Friend)
							{
								S.GossipBonus++;
							}
							if (S.StudentManager.EmbarassingSecret && S.DialogueWheel.Victim == S.StudentManager.RivalID)
							{
								S.GossipBonus++;
							}
							if ((S.Male && S.Yandere.Class.Seduction + S.Yandere.Class.SeductionBonus > 0) || S.Yandere.Class.Seduction == 5)
							{
								S.GossipBonus++;
							}
							if (S.Reputation.Reputation > 33.33333f)
							{
								S.GossipBonus++;
							}
							if (S.Club == ClubType.Bully)
							{
								S.GossipBonus++;
							}
							S.GossipBonus += S.Yandere.Class.PsychologyGrade + S.Yandere.Class.PsychologyBonus;
							S.StudentManager.StudentReps[S.DialogueWheel.Victim] -= 1 + S.GossipBonus;
							if (S.Club != ClubType.Bully)
							{
								S.Reputation.PendingRep -= 2f;
								S.PendingRep -= 2f;
							}
							S.Gossiped = true;
							S.Yandere.NotificationManager.TopicName = "Gossip";
							if (S.StudentManager.Students[S.DialogueWheel.Victim] != null)
							{
								S.Yandere.NotificationManager.CustomText = S.StudentManager.Students[S.DialogueWheel.Victim].Name + "'s rep is now " + S.StudentManager.StudentReps[S.DialogueWheel.Victim];
								S.Yandere.NotificationManager.DisplayNotification(NotificationType.Custom);
							}
						}
						else
						{
							S.Subtitle.PersonaSubtitle.UpdateLabel(PersonaType.None, S.Reputation.Reputation, 5f);
							S.Reputation.PendingRep -= 1f;
							S.PendingRep -= 1f;
						}
						int topicSelected2 = S.StudentManager.DialogueWheel.TopicInterface.TopicSelected;
						if (!S.StudentManager.GetTopicLearnedByStudent(topicSelected2, S.StudentID))
						{
							S.Yandere.NotificationManager.TopicName = S.StudentManager.InterestManager.TopicNames[topicSelected2];
							S.Yandere.NotificationManager.DisplayNotification(NotificationType.Opinion);
							S.StudentManager.SetTopicLearnedByStudent(topicSelected2, S.StudentID, boolean: true);
						}
						if (!S.StudentManager.GetTopicLearnedByStudent(19, S.StudentID))
						{
							S.Yandere.NotificationManager.TopicName = "Gossip";
							S.Yandere.NotificationManager.DisplayNotification(NotificationType.Opinion);
							S.StudentManager.SetTopicLearnedByStudent(19, S.StudentID, boolean: true);
						}
					}
				}
				else
				{
					S.Subtitle.UpdateLabel(SubtitleType.Dismissive, 2, 3f);
				}
			}
			else
			{
				if (Input.GetButtonDown(InputNames.Xbox_A))
				{
					S.TalkTimer = 0f;
				}
				if (S.CharacterAnimation[S.GossipAnim].time >= S.CharacterAnimation[S.GossipAnim].length)
				{
					S.CharacterAnimation.CrossFade(IdleAnim);
				}
				if (S.TalkTimer <= 0f)
				{
					S.DialogueWheel.End();
				}
			}
			S.TalkTimer -= Time.deltaTime;
		}
		else if (S.Interaction == StudentInteractionType.Bye)
		{
			if (S.TalkTimer == 2f)
			{
				if (S.Club != ClubType.Delinquent)
				{
					if (S.DialogueWheel.Social.Socialized)
					{
						S.Subtitle.UpdateLabel(SubtitleType.StudentFarewell, 1, 2f);
					}
					else
					{
						S.Subtitle.UpdateLabel(SubtitleType.StudentFarewell, 0, 2f);
					}
				}
				else
				{
					S.Subtitle.UpdateLabel(SubtitleType.Dismissive, 3, 3f);
				}
			}
			else if (Input.GetButtonDown(InputNames.Xbox_A))
			{
				S.TalkTimer = 0f;
			}
			S.CharacterAnimation.CrossFade(IdleAnim);
			S.TalkTimer -= Time.deltaTime;
			if (S.TalkTimer <= 0f)
			{
				if (!S.DialogueWheel.Social.Socialized)
				{
					S.Pestered += 2;
				}
				S.DialogueWheel.End();
			}
		}
		else if (S.Interaction == StudentInteractionType.GivingTask)
		{
			if (S.TalkTimer == 100f)
			{
				bool flag = false;
				bool flag2 = true;
				if (S.Subtitle.TaskRequirements[S.StudentID] != "")
				{
					Debug.Log("This character has a requirement to offer a Task.");
					S.StudentManager.TaskManager.CheckTaskRequirement(S.StudentID);
					flag2 = S.StudentManager.TaskManager.Proceed;
					flag = S.StudentManager.TaskManager.Impossible;
				}
				if (flag)
				{
					S.TaskPhase = 999;
					S.Subtitle.CustomText = "I'm sorry...I don't think there is anything you can help me with.";
					S.Subtitle.UpdateLabel(SubtitleType.Custom, S.StudentID, 10f);
					S.Subtitle.Timer = 0f;
					S.CharacterAnimation.CrossFade(S.TaskAnims[1]);
					S.CurrentAnim = S.TaskAnims[1];
					S.TalkTimer = 10f;
				}
				else if (flag2)
				{
					S.Subtitle.UpdateLabel(S.TaskLineResponseType, S.TaskPhase, S.Subtitle.GetClipLength(S.StudentID, S.TaskPhase));
					S.CharacterAnimation.CrossFade(S.TaskAnims[S.TaskPhase]);
					S.CurrentAnim = S.TaskAnims[S.TaskPhase];
					S.TalkTimer = S.Subtitle.GetClipLength(S.StudentID, S.TaskPhase);
				}
				else
				{
					S.TaskPhase = 999;
					S.Subtitle.UpdateLabel(SubtitleType.TaskRequirement, S.StudentID, 10f);
					S.Subtitle.Timer = 0f;
					S.CharacterAnimation.CrossFade(S.TaskAnims[1]);
					S.CurrentAnim = S.TaskAnims[1];
					S.TalkTimer = 10f;
				}
			}
			else if (Input.GetButtonDown(InputNames.Xbox_A))
			{
				S.Subtitle.Label.text = string.Empty;
				Object.Destroy(S.Subtitle.CurrentClip);
				S.TalkTimer = 0f;
			}
			if (S.CurrentAnim != "" && S.CharacterAnimation[S.CurrentAnim].time >= S.CharacterAnimation[S.CurrentAnim].length)
			{
				S.CharacterAnimation.CrossFade(IdleAnim);
			}
			S.TalkTimer -= Time.deltaTime;
			if (S.TalkTimer <= 0f)
			{
				if (S.TaskPhase == 5)
				{
					if (!ConversationGlobals.GetTopicDiscovered(21))
					{
						S.Yandere.NotificationManager.TopicName = "Solitude";
						S.Yandere.NotificationManager.DisplayNotification(NotificationType.Topic);
						ConversationGlobals.SetTopicDiscovered(21, value: true);
					}
					if (!S.StudentManager.GetTopicLearnedByStudent(21, S.StudentID))
					{
						S.Yandere.NotificationManager.TopicName = "Solitude";
						S.Yandere.NotificationManager.DisplayNotification(NotificationType.Opinion);
						S.StudentManager.SetTopicLearnedByStudent(21, S.StudentID, boolean: true);
					}
					S.DialogueWheel.TaskWindow.TaskComplete = true;
					S.StudentManager.TaskManager.TaskStatus[S.StudentID] = 3;
					S.Interaction = StudentInteractionType.Idle;
					S.DialogueWheel.Social.Student = S;
					S.DialogueWheel.Social.StudentID = S.StudentID;
					S.DialogueWheel.Social.StudentFriendships[S.StudentID] += 50;
					S.DialogueWheel.Social.CheckFriendStatus();
					if (S.Club != ClubType.Delinquent)
					{
						CalculateRepBonus();
						S.Reputation.PendingRep += 1f + (float)S.RepBonus;
						S.PendingRep += 1f + (float)S.RepBonus;
					}
					else
					{
						S.StudentManager.DelinquentVoices.SetActive(value: false);
						S.RespectEarned = true;
					}
					if (SchemeGlobals.GetSchemeStage(6) == 3)
					{
						SchemeGlobals.SetSchemeStage(6, 4);
						S.Yandere.PauseScreen.Schemes.UpdateInstructions();
					}
				}
				else if (S.TaskPhase == 4 || S.TaskPhase == 0)
				{
					S.StudentManager.TaskManager.UpdateTaskStatus();
					S.DialogueWheel.End();
				}
				else if (S.TaskPhase == 3)
				{
					S.DialogueWheel.TaskWindow.UpdateWindow(S.StudentID);
					S.Subtitle.Label.text = "";
					S.Interaction = StudentInteractionType.Idle;
				}
				else if (S.TaskPhase == 999)
				{
					S.TaskPhase = 0;
					S.Interaction = StudentInteractionType.Idle;
					S.DialogueWheel.End();
				}
				else
				{
					S.TaskPhase++;
					S.Subtitle.UpdateLabel(S.TaskLineResponseType, S.TaskPhase, S.Subtitle.GetClipLength(S.StudentID, S.TaskPhase));
					S.Subtitle.Timer = 0f;
					S.CharacterAnimation.CrossFade(S.TaskAnims[S.TaskPhase]);
					S.CurrentAnim = S.TaskAnims[S.TaskPhase];
					S.TalkTimer = S.Subtitle.GetClipLength(S.StudentID, S.TaskPhase);
				}
			}
		}
		else if (S.Interaction == StudentInteractionType.FollowingPlayer)
		{
			if (S.TalkTimer == 2f)
			{
				if (S.Club != ClubType.Delinquent)
				{
					bool flag3 = false;
					bool flag4 = false;
					bool flag5 = false;
					if (S.StudentID == S.StudentManager.RivalID)
					{
						if (S.Follower != null && S.Follower.CurrentAction == StudentActionType.Follow && !S.Follower.Distracting && !S.Follower.GoAway && !S.Follower.EatingSnack && S.gameObject.activeInHierarchy)
						{
							flag3 = true;
						}
						if (S.CurrentAction == StudentActionType.SitAndEatBento)
						{
							flag4 = true;
						}
					}
					if ((S.Clock.HourTime > 8f && S.Clock.HourTime < 13f) || (S.Clock.HourTime > 13.375f && S.Clock.HourTime < 15.5f) || (S.StudentID == S.StudentManager.RivalID && flag3) || (S.StudentID == S.StudentManager.RivalID && flag5) || (S.StudentID == S.StudentManager.RivalID && flag4) || (!S.StudentManager.MissionMode && SchoolGlobals.SchoolAtmosphere <= 0.5f) || S.CurrentDestination == S.Seat || S.Schoolwear == 2 || !S.Indoors)
					{
						Debug.Log("Current Clock.HourTime is: " + S.Clock.HourTime);
						S.CharacterAnimation.CrossFade(S.GossipAnim);
						NegativeResponse = true;
						if (S.StudentID == S.StudentManager.RivalID)
						{
							if (S.Yandere.Club == ClubType.Delinquent)
							{
								S.Subtitle.CustomText = "Are you trying to intimidate me? Well, it won't work! Go away!";
								S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
								S.TalkTimer = 5f;
							}
							else if (flag3)
							{
								S.Subtitle.UpdateLabel(SubtitleType.StudentStay, 2, 5f);
								S.TalkTimer = 5f;
							}
							else if (flag5)
							{
								S.Subtitle.UpdateLabel(SubtitleType.StudentStay, 3, 13f);
								S.TalkTimer = 13f;
							}
							else if (flag4)
							{
								S.Subtitle.CustomText = "Now? I'm busy eating...can you show me later, instead?";
								S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
								S.TalkTimer = 5f;
							}
							else if (SchoolGlobals.SchoolAtmosphere <= 0.5f)
							{
								S.Subtitle.CustomText = "I wouldn't be comfortable with that...the school doesn't feel safe right now.";
								S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
								S.TalkTimer = 5f;
							}
							else if (S.Schoolwear == 2)
							{
								S.Subtitle.CustomText = "Uh...I'm wearing a swimsuit right now. I can't really do that for you. Maybe later...?";
								S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 7.5f);
								S.TalkTimer = 7.5f;
								Refuse = true;
							}
							else if (S.CurrentDestination == S.Seat)
							{
								S.Subtitle.CustomText = "Can it wait? I need to do something real quick.";
								S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
								S.TalkTimer = 5f;
							}
							else if (!S.Indoors)
							{
								S.Subtitle.CustomText = "Sorry, it's a bit early in the day for that. At least wait until I've changed my shoes.";
								S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
								S.TalkTimer = 5f;
							}
							else
							{
								Debug.Log("We got to THIS part of the code?!");
								S.Subtitle.UpdateLabel(SubtitleType.StudentStay, 0, 5f);
								S.TalkTimer = 5f;
								Refuse = true;
							}
						}
						else if (S.Schoolwear == 2)
						{
							S.Subtitle.CustomText = "Uh...I'm wearing a swimsuit right now. I can't really do that for you. Maybe later...?";
							S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 7.5f);
							S.TalkTimer = 7.5f;
							Refuse = true;
						}
						else if (SchoolGlobals.SchoolAtmosphere <= 0.5f)
						{
							S.Subtitle.CustomText = "I wouldn't be comfortable with that...the school doesn't feel safe right now.";
							S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 7.5f);
							S.TalkTimer = 5f;
							Refuse = true;
						}
						else if (!S.Indoors)
						{
							S.Subtitle.CustomText = "Sorry, it's a bit early in the day for that. At least wait until I've changed my shoes.";
							S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
							S.TalkTimer = 5f;
						}
						else
						{
							S.Subtitle.UpdateLabel(SubtitleType.StudentStay, 0, 5f);
							S.TalkTimer = 5f;
							Refuse = true;
						}
					}
					else if (S.StudentManager.LockerRoomArea.bounds.Contains(S.Yandere.transform.position) || S.StudentManager.WestBathroomArea.bounds.Contains(S.Yandere.transform.position) || S.StudentManager.EastBathroomArea.bounds.Contains(S.Yandere.transform.position) || S.StudentManager.HeadmasterArea.bounds.Contains(S.Yandere.transform.position) || S.MyRenderer.sharedMesh == S.SchoolSwimsuit || S.MyRenderer.sharedMesh == S.SwimmingTrunks || S.Traumatized)
					{
						S.CharacterAnimation.CrossFade(S.GossipAnim);
						S.Subtitle.UpdateLabel(SubtitleType.StudentStay, 1, 5f);
						NegativeResponse = true;
					}
					else
					{
						int num = 0;
						if (S.Yandere.Club == ClubType.Delinquent)
						{
							S.Reputation.PendingRep -= 10f;
							S.PendingRep -= 10f;
							num++;
						}
						S.CharacterAnimation.CrossFade(S.Nod1Anim);
						S.Subtitle.UpdateLabel(SubtitleType.StudentFollow, num, 2f);
						Follow = true;
					}
				}
				else
				{
					S.Subtitle.UpdateLabel(SubtitleType.Dismissive, 4, 5f);
				}
			}
			else
			{
				if (Input.GetButtonDown(InputNames.Xbox_A))
				{
					S.TalkTimer = 0f;
				}
				if (S.CharacterAnimation[S.Nod1Anim].time >= S.CharacterAnimation[S.Nod1Anim].length)
				{
					S.CharacterAnimation.CrossFade(IdleAnim);
				}
				if (S.TalkTimer <= 0f)
				{
					S.DialogueWheel.End();
					if (Follow)
					{
						S.Pathfinding.target = S.Yandere.transform;
						S.Prompt.Label[0].text = "     Stop";
						if (S.StudentID == 30)
						{
							S.StudentManager.FollowerLookAtTarget.position = S.DefaultTarget.position;
							S.StudentManager.LoveManager.Follower = S;
						}
						S.FollowCountdown.Sprite.fillAmount = 1f;
						if (S.Yandere.Club != ClubType.Delinquent)
						{
							S.FollowCountdown.Speed = 1f / (35f + S.Reputation.Reputation * 0.25f);
						}
						else
						{
							S.FollowCountdown.Speed = 1f / (35f + S.Reputation.Reputation * -0.25f);
						}
						S.FollowCountdown.gameObject.SetActive(value: true);
						S.Yandere.Follower = S;
						S.Yandere.Followers++;
						S.Following = true;
						S.Hurry = false;
						S.StudentManager.InterestManager.FollowerID = S.StudentID;
						S.StudentManager.InterestManager.UpdateIgnore();
					}
					Follow = false;
				}
			}
			S.TalkTimer -= Time.deltaTime;
		}
		else if (S.Interaction == StudentInteractionType.GoingAway)
		{
			if (S.TalkTimer == 3f)
			{
				if (S.Club != ClubType.Delinquent)
				{
					bool flag6 = false;
					bool flag7 = false;
					if (S.StudentID == S.StudentManager.RivalID && S.CurrentAction == StudentActionType.SitAndEatBento)
					{
						flag6 = true;
					}
					if ((S.Clock.HourTime > 8f && S.Clock.HourTime < 13f) || (S.Clock.HourTime > 13.375f && S.Clock.HourTime < 15.5f) || SchoolGlobals.SchoolAtmosphere <= 0.5f || S.Schoolwear == 2 || (S.StudentID == S.StudentManager.RivalID && flag6) || (S.StudentID == S.StudentManager.RivalID && flag7) || S.CurrentDestination == S.Seat || !S.Indoors)
					{
						S.CharacterAnimation.CrossFade(S.GossipAnim);
						Refuse = true;
						if (S.Schoolwear == 2)
						{
							S.Subtitle.CustomText = "Uh...I'm wearing a swimsuit right now. I can't really do that for you. Maybe later...?";
							S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 7.5f);
							S.TalkTimer = 7.5f;
						}
						else if (SchoolGlobals.SchoolAtmosphere <= 0.5f)
						{
							S.Subtitle.CustomText = "I'm sorry, I wouldn't be comfortable with that...I'm not even sure if we're safe right now.";
							S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 7.5f);
							S.TalkTimer = 7.5f;
						}
						else if (flag6)
						{
							S.Subtitle.CustomText = "Ugh...now? Seriously? I'm busy eating...can it wait?";
							S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 7.5f);
							S.TalkTimer = 7.5f;
						}
						else if (flag7)
						{
							S.Subtitle.UpdateLabel(SubtitleType.StudentStay, 3, 13f);
							S.TalkTimer = 13f;
						}
						else if (S.CurrentDestination == S.Seat)
						{
							S.Subtitle.CustomText = "Can it wait? I need to do something real quick.";
							S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
							S.TalkTimer = 5f;
						}
						else if (!S.Indoors)
						{
							S.Subtitle.CustomText = "Sorry, it's a bit early in the day for that. At least wait until I've changed my shoes.";
							S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
							S.TalkTimer = 5f;
						}
						else
						{
							S.Subtitle.UpdateLabel(SubtitleType.StudentStay, 0, 5f);
							S.TalkTimer = 5f;
						}
					}
					else
					{
						int num2 = 0;
						if (S.Yandere.Club == ClubType.Delinquent)
						{
							S.Reputation.PendingRep -= 10f;
							S.PendingRep -= 10f;
							num2++;
						}
						S.CharacterAnimation.CrossFade(S.Nod1Anim);
						S.Subtitle.UpdateLabel(SubtitleType.StudentLeave, num2, 3f);
						S.GoAway = true;
					}
				}
				else
				{
					S.Subtitle.UpdateLabel(SubtitleType.Dismissive, 5, 5f);
				}
			}
			else
			{
				if (Input.GetButtonDown(InputNames.Xbox_A))
				{
					S.TalkTimer = 0f;
				}
				if (S.CharacterAnimation[S.Nod1Anim].time >= S.CharacterAnimation[S.Nod1Anim].length)
				{
					S.CharacterAnimation.CrossFade(IdleAnim);
				}
				if (S.TalkTimer <= 0f)
				{
					S.DialogueWheel.End();
				}
			}
			S.TalkTimer -= Time.deltaTime;
		}
		else if (S.Interaction == StudentInteractionType.DistractingTarget)
		{
			if (S.TalkTimer == 3f)
			{
				if (S.Club != ClubType.Delinquent)
				{
					bool flag8 = false;
					bool flag9 = false;
					if (S.StudentID == S.StudentManager.RivalID && S.CurrentAction == StudentActionType.SitAndEatBento)
					{
						flag8 = true;
					}
					if ((S.Clock.HourTime > 8f && S.Clock.HourTime < 13f) || (S.Clock.HourTime > 13.375f && S.Clock.HourTime < 15.5f) || SchoolGlobals.SchoolAtmosphere <= 0.5f || S.Schoolwear == 2 || (S.StudentID == S.StudentManager.RivalID && flag8) || (S.StudentID == S.StudentManager.RivalID && flag9) || S.CurrentDestination == S.Seat || !S.Indoors)
					{
						S.CharacterAnimation.CrossFade(S.GossipAnim);
						Refuse = true;
						if (S.Schoolwear == 2)
						{
							S.Subtitle.CustomText = "Uh...I'm wearing a swimsuit right now. I can't really do that for you. Maybe later...?";
							S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 7.5f);
							S.TalkTimer = 7.5f;
						}
						else if (SchoolGlobals.SchoolAtmosphere <= 0.5f)
						{
							S.Subtitle.CustomText = "I'm sorry, I wouldn't be comfortable with that...I'm not even sure if we're safe right now.";
							S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 7.5f);
							S.TalkTimer = 7.5f;
						}
						else if (flag8)
						{
							S.Subtitle.CustomText = "I'm sorry, but could I please do it later? I'm eating a meal right now...";
							S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 7.5f);
							S.TalkTimer = 7.5f;
						}
						else if (flag9)
						{
							S.Subtitle.UpdateLabel(SubtitleType.StudentStay, 3, 13f);
							S.TalkTimer = 13f;
						}
						else if (S.CurrentDestination == S.Seat)
						{
							S.Subtitle.CustomText = "Can it wait? I need to do something real quick.";
							S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
							S.TalkTimer = 5f;
						}
						else if (!S.Indoors)
						{
							S.Subtitle.CustomText = "Sorry, it's a bit early in the day for that. At least wait until I've changed my shoes.";
							S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
							S.TalkTimer = 5f;
						}
						else
						{
							S.Subtitle.UpdateLabel(SubtitleType.StudentStay, 0, 5f);
							S.TalkTimer = 5f;
						}
					}
					else
					{
						StudentScript studentScript = S.StudentManager.Students[S.DialogueWheel.Victim];
						Grudge = false;
						if (studentScript.Club == ClubType.Delinquent || (S.Bullied && studentScript.Club == ClubType.Bully) || (studentScript.StudentID == 36 && S.StudentManager.TaskManager.TaskStatus[36] < 3))
						{
							Grudge = true;
						}
						if (studentScript.Routine && !studentScript.TargetedForDistraction && !studentScript.InEvent && !Grudge && studentScript.Indoors && studentScript.gameObject.activeInHierarchy && studentScript.ClubActivityPhase < 16 && studentScript.CurrentAction != StudentActionType.Sunbathe)
						{
							int num3 = 0;
							if (S.Yandere.Club == ClubType.Delinquent)
							{
								S.Reputation.PendingRep -= 10f;
								S.PendingRep -= 10f;
								num3++;
							}
							S.CharacterAnimation.CrossFade(S.Nod1Anim);
							S.Subtitle.UpdateLabel(SubtitleType.StudentDistract, num3, 3f);
							Refuse = false;
						}
						else
						{
							S.CharacterAnimation.CrossFade(S.GossipAnim);
							if (Grudge)
							{
								S.Subtitle.UpdateLabel(SubtitleType.StudentDistractBullyRefuse, 0, 3f);
							}
							else
							{
								S.Subtitle.UpdateLabel(SubtitleType.StudentDistractRefuse, 0, 3f);
							}
							Refuse = true;
						}
					}
				}
				else
				{
					S.Subtitle.UpdateLabel(SubtitleType.Dismissive, 6, 5f);
					Refuse = true;
				}
			}
			else
			{
				if (Input.GetButtonDown(InputNames.Xbox_A))
				{
					S.TalkTimer = 0f;
				}
				if (S.CharacterAnimation[S.Nod1Anim].time >= S.CharacterAnimation[S.Nod1Anim].length)
				{
					S.CharacterAnimation.CrossFade(IdleAnim);
				}
				if (S.TalkTimer <= 0f)
				{
					S.DialogueWheel.End();
					if (!Refuse && (S.Clock.HourTime < 8f || (S.Clock.HourTime > 13f && S.Clock.HourTime < 13.375f) || S.Clock.HourTime > 15.5f) && !S.Distracting)
					{
						S.DistractionTarget = S.StudentManager.Students[S.DialogueWheel.Victim];
						S.DistractionTarget.TargetedForDistraction = true;
						S.CurrentDestination = S.DistractionTarget.transform;
						S.Pathfinding.target = S.DistractionTarget.transform;
						S.Pathfinding.speed = 4f;
						S.TargetDistance = 1f;
						S.DistractTimer = 10f;
						S.Distracting = true;
						S.Routine = false;
						S.CanTalk = false;
					}
				}
			}
			S.TalkTimer -= Time.deltaTime;
		}
		else if (S.Interaction == StudentInteractionType.PersonalGrudge)
		{
			if (S.TalkTimer == 5f)
			{
				if (S.Persona == PersonaType.Coward || S.Persona == PersonaType.Fragile)
				{
					S.Subtitle.UpdateLabel(SubtitleType.CowardGrudge, 0, 5f);
					S.CharacterAnimation.CrossFade(S.CowardAnim);
					S.TalkTimer = 5f;
				}
				else
				{
					if (!S.Male)
					{
						S.Subtitle.UpdateLabel(SubtitleType.GrudgeWarning, 0, 99f);
					}
					else if (S.Club == ClubType.Delinquent)
					{
						S.Subtitle.UpdateLabel(SubtitleType.DelinquentGrudge, 1, 99f);
					}
					else
					{
						S.Subtitle.UpdateLabel(SubtitleType.GrudgeWarning, 1, 99f);
					}
					S.TalkTimer = S.Subtitle.CurrentClip.GetComponent<AudioSource>().clip.length;
					S.CharacterAnimation.CrossFade(S.GrudgeAnim);
				}
			}
			else
			{
				if (Input.GetButtonDown(InputNames.Xbox_A))
				{
					S.TalkTimer = 0f;
				}
				if (S.TalkTimer <= 0f)
				{
					S.DialogueWheel.End();
				}
			}
			S.TalkTimer -= Time.deltaTime;
		}
		else if (S.Interaction == StudentInteractionType.ClubInfo)
		{
			if (S.TalkTimer == 100f)
			{
				S.Subtitle.UpdateLabel(S.ClubInfoResponseType, S.ClubPhase, 99f);
				S.TalkTimer = S.Subtitle.GetClubClipLength(S.Club, S.ClubPhase);
			}
			else if (Input.GetButtonDown(InputNames.Xbox_A))
			{
				S.Subtitle.Label.text = string.Empty;
				Object.Destroy(S.Subtitle.CurrentClip);
				S.TalkTimer = 0f;
			}
			S.TalkTimer -= Time.deltaTime;
			if (S.TalkTimer <= 0f)
			{
				if (S.ClubPhase == 3)
				{
					S.DialogueWheel.Panel.enabled = true;
					S.DialogueWheel.Show = true;
					S.Subtitle.Label.text = string.Empty;
					S.Interaction = StudentInteractionType.Idle;
					S.TalkTimer = 0f;
				}
				else
				{
					S.ClubPhase++;
					S.Subtitle.UpdateLabel(S.ClubInfoResponseType, S.ClubPhase, 99f);
					S.TalkTimer = S.Subtitle.GetClubClipLength(S.Club, S.ClubPhase);
				}
			}
		}
		else if (S.Interaction == StudentInteractionType.ClubJoin)
		{
			if (S.TalkTimer == 100f)
			{
				if (S.ClubPhase == 1)
				{
					S.Subtitle.UpdateLabel(SubtitleType.ClubJoin, (int)(S.Club + ClubBonus), 99f);
					S.TalkTimer = S.Subtitle.CurrentClip.GetComponent<AudioSource>().clip.length;
				}
				else if (S.ClubPhase == 2)
				{
					S.Subtitle.UpdateLabel(SubtitleType.ClubAccept, (int)(S.Club + ClubBonus), 99f);
					S.TalkTimer = S.Subtitle.CurrentClip.GetComponent<AudioSource>().clip.length;
				}
				else if (S.ClubPhase == 3)
				{
					S.Subtitle.UpdateLabel(SubtitleType.ClubRefuse, (int)(S.Club + ClubBonus), 99f);
					S.TalkTimer = S.Subtitle.CurrentClip.GetComponent<AudioSource>().clip.length;
				}
				else if (S.ClubPhase == 4)
				{
					S.Subtitle.UpdateLabel(SubtitleType.ClubRejoin, (int)(S.Club + ClubBonus), 99f);
					S.TalkTimer = S.Subtitle.CurrentClip.GetComponent<AudioSource>().clip.length;
				}
				else if (S.ClubPhase == 5)
				{
					S.Subtitle.UpdateLabel(SubtitleType.ClubExclusive, (int)(S.Club + ClubBonus), 99f);
					S.TalkTimer = S.Subtitle.CurrentClip.GetComponent<AudioSource>().clip.length;
				}
				else if (S.ClubPhase == 6)
				{
					S.Subtitle.UpdateLabel(SubtitleType.ClubGrudge, (int)(S.Club + ClubBonus), 99f);
					S.TalkTimer = S.Subtitle.CurrentClip.GetComponent<AudioSource>().clip.length;
				}
			}
			else if (Input.GetButtonDown(InputNames.Xbox_A))
			{
				S.Subtitle.Label.text = string.Empty;
				Object.Destroy(S.Subtitle.CurrentClip);
				S.TalkTimer = 0f;
			}
			S.TalkTimer -= Time.deltaTime;
			if (S.TalkTimer <= 0f)
			{
				if (S.ClubPhase == 1)
				{
					S.DialogueWheel.ClubWindow.Club = S.Club;
					S.DialogueWheel.ClubWindow.UpdateWindow();
					S.Subtitle.Label.text = string.Empty;
					S.Interaction = StudentInteractionType.Idle;
				}
				else
				{
					S.DialogueWheel.End();
					if (S.Club == ClubType.MartialArts)
					{
						S.ChangingBooth.CheckYandereClub();
					}
				}
			}
		}
		else if (S.Interaction == StudentInteractionType.ClubQuit)
		{
			if (S.TalkTimer == 100f)
			{
				if (S.ClubPhase == 1)
				{
					S.Subtitle.UpdateLabel(SubtitleType.ClubQuit, (int)(S.Club + ClubBonus), 99f);
					S.TalkTimer = S.Subtitle.CurrentClip.GetComponent<AudioSource>().clip.length;
				}
				else if (S.ClubPhase == 2)
				{
					S.Subtitle.UpdateLabel(SubtitleType.ClubConfirm, (int)(S.Club + ClubBonus), 99f);
					S.TalkTimer = S.Subtitle.CurrentClip.GetComponent<AudioSource>().clip.length;
				}
				else if (S.ClubPhase == 3)
				{
					S.Subtitle.UpdateLabel(SubtitleType.ClubDeny, (int)(S.Club + ClubBonus), 99f);
					S.TalkTimer = S.Subtitle.CurrentClip.GetComponent<AudioSource>().clip.length;
				}
			}
			else if (Input.GetButtonDown(InputNames.Xbox_A))
			{
				S.Subtitle.Label.text = string.Empty;
				Object.Destroy(S.Subtitle.CurrentClip);
				S.TalkTimer = 0f;
			}
			S.TalkTimer -= Time.deltaTime;
			if (S.TalkTimer <= 0f)
			{
				if (S.ClubPhase == 1)
				{
					S.DialogueWheel.ClubWindow.Club = S.Club;
					S.DialogueWheel.ClubWindow.Quitting = true;
					S.DialogueWheel.ClubWindow.UpdateWindow();
					S.Subtitle.Label.text = string.Empty;
					S.Interaction = StudentInteractionType.Idle;
				}
				else
				{
					S.DialogueWheel.End();
					if (S.Club == ClubType.MartialArts)
					{
						S.ChangingBooth.CheckYandereClub();
					}
					if (S.ClubPhase != 2)
					{
					}
				}
			}
		}
		else if (S.Interaction == StudentInteractionType.ClubBye)
		{
			if (S.TalkTimer == S.Subtitle.ClubFarewellClips[(int)(S.Club + ClubBonus)].length)
			{
				S.Subtitle.UpdateLabel(SubtitleType.ClubFarewell, (int)(S.Club + ClubBonus), S.Subtitle.ClubFarewellClips[(int)(S.Club + ClubBonus)].length);
			}
			else if (Input.GetButtonDown(InputNames.Xbox_A))
			{
				S.TalkTimer = 0f;
			}
			S.TalkTimer -= Time.deltaTime;
			if (S.TalkTimer <= 0f)
			{
				S.DialogueWheel.End();
			}
		}
		else if (S.Interaction == StudentInteractionType.ClubActivity)
		{
			if (S.TalkTimer == 100f)
			{
				if (S.ClubPhase == 1)
				{
					S.Subtitle.UpdateLabel(SubtitleType.ClubActivity, (int)(S.Club + ClubBonus), 99f);
					S.TalkTimer = S.Subtitle.CurrentClip.GetComponent<AudioSource>().clip.length;
				}
				else if (S.ClubPhase == 2)
				{
					S.Subtitle.UpdateLabel(SubtitleType.ClubYes, (int)(S.Club + ClubBonus), 99f);
					S.TalkTimer = S.Subtitle.CurrentClip.GetComponent<AudioSource>().clip.length;
				}
				else if (S.ClubPhase == 3)
				{
					S.Subtitle.UpdateLabel(SubtitleType.ClubNo, (int)(S.Club + ClubBonus), 99f);
					S.TalkTimer = S.Subtitle.CurrentClip.GetComponent<AudioSource>().clip.length;
				}
				else if (S.ClubPhase == 4)
				{
					S.Subtitle.UpdateLabel(SubtitleType.ClubEarly, (int)(S.Club + ClubBonus), 99f);
					S.TalkTimer = S.Subtitle.CurrentClip.GetComponent<AudioSource>().clip.length;
				}
				else if (S.ClubPhase == 5)
				{
					S.Subtitle.UpdateLabel(SubtitleType.ClubLate, (int)(S.Club + ClubBonus), 99f);
					S.TalkTimer = S.Subtitle.CurrentClip.GetComponent<AudioSource>().clip.length;
				}
			}
			else if (Input.GetButtonDown(InputNames.Xbox_A))
			{
				S.Subtitle.Label.text = string.Empty;
				Object.Destroy(S.Subtitle.CurrentClip);
				S.TalkTimer = 0f;
			}
			S.TalkTimer -= Time.deltaTime;
			if (S.TalkTimer <= 0f)
			{
				if (S.ClubPhase == 1)
				{
					S.DialogueWheel.ClubWindow.Club = S.Club;
					S.DialogueWheel.ClubWindow.Activity = true;
					S.DialogueWheel.ClubWindow.UpdateWindow();
					S.Subtitle.Label.text = string.Empty;
					S.Interaction = StudentInteractionType.Idle;
				}
				else if (S.ClubPhase == 2)
				{
					S.Police.Darkness.enabled = true;
					S.Police.ClubActivity = true;
					S.Police.FadeOut = true;
					S.Subtitle.Label.text = string.Empty;
					S.Interaction = StudentInteractionType.Idle;
				}
				else
				{
					S.DialogueWheel.End();
				}
			}
		}
		else if (S.Interaction == StudentInteractionType.ClubUnwelcome)
		{
			S.CharacterAnimation.CrossFade(S.IdleAnim);
			if (S.TalkTimer == 5f)
			{
				S.Subtitle.UpdateLabel(SubtitleType.ClubUnwelcome, (int)(S.Club + ClubBonus), 99f);
				S.TalkTimer = S.Subtitle.CurrentClip.GetComponent<AudioSource>().clip.length;
			}
			else
			{
				if (Input.GetButtonDown(InputNames.Xbox_A))
				{
					S.TalkTimer = 0f;
				}
				if (S.TalkTimer <= 0f)
				{
					S.DialogueWheel.End();
				}
			}
			S.TalkTimer -= Time.deltaTime;
		}
		else if (S.Interaction == StudentInteractionType.ClubKick)
		{
			S.CharacterAnimation.CrossFade(S.IdleAnim);
			if (S.TalkTimer == 5f)
			{
				S.Subtitle.UpdateLabel(SubtitleType.ClubKick, (int)(S.Club + ClubBonus), 99f);
				S.TalkTimer = S.Subtitle.CurrentClip.GetComponent<AudioSource>().clip.length;
			}
			else
			{
				if (Input.GetButtonDown(InputNames.Xbox_A))
				{
					S.TalkTimer = 0f;
				}
				if (S.TalkTimer <= 0f)
				{
					S.ClubManager.DeactivateClubBenefit();
					S.Yandere.Club = ClubType.None;
					S.DialogueWheel.End();
					S.Yandere.ClubAccessory();
				}
			}
			S.TalkTimer -= Time.deltaTime;
		}
		else if (S.Interaction == StudentInteractionType.ClubGrudge)
		{
			S.CharacterAnimation.CrossFade(S.IdleAnim);
			if (S.TalkTimer == 5f)
			{
				if (S.ClubManager.ClubGrudge)
				{
					S.Subtitle.UpdateLabel(SubtitleType.ClubGrudge, (int)(S.Club + ClubBonus), 99f);
				}
				else
				{
					S.Subtitle.CustomText = "You joined our club, and then you just...never showed up. I don't see a reason to let you join the club again...";
					S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 99f);
				}
				S.TalkTimer = 10f;
			}
			else
			{
				if (Input.GetButtonDown(InputNames.Xbox_A))
				{
					S.TalkTimer = 0f;
				}
				if (S.TalkTimer <= 0f)
				{
					S.DialogueWheel.End();
				}
			}
			S.TalkTimer -= Time.deltaTime;
		}
		else if (S.Interaction == StudentInteractionType.ClubPractice)
		{
			if (S.TalkTimer == 100f)
			{
				if (S.ClubPhase == 1)
				{
					S.Subtitle.UpdateLabel(SubtitleType.ClubPractice, (int)(S.Club + ClubBonus), 99f);
					S.TalkTimer = S.Subtitle.CurrentClip.GetComponent<AudioSource>().clip.length;
				}
				else if (S.ClubPhase == 2)
				{
					S.Subtitle.UpdateLabel(SubtitleType.ClubPracticeYes, (int)(S.Club + ClubBonus), 99f);
					S.TalkTimer = S.Subtitle.CurrentClip.GetComponent<AudioSource>().clip.length;
				}
				else if (S.ClubPhase == 3)
				{
					S.Subtitle.UpdateLabel(SubtitleType.ClubPracticeNo, (int)(S.Club + ClubBonus), 99f);
					S.TalkTimer = S.Subtitle.CurrentClip.GetComponent<AudioSource>().clip.length;
				}
			}
			else if (Input.GetButtonDown(InputNames.Xbox_A))
			{
				S.Subtitle.Label.text = string.Empty;
				Object.Destroy(S.Subtitle.CurrentClip);
				S.TalkTimer = 0f;
			}
			S.TalkTimer -= Time.deltaTime;
			if (S.TalkTimer <= 0f)
			{
				if (S.ClubPhase == 1)
				{
					S.DialogueWheel.PracticeWindow.Club = S.Club;
					S.DialogueWheel.PracticeWindow.UpdateWindow();
					S.DialogueWheel.PracticeWindow.Selected = 1;
					S.DialogueWheel.PracticeWindow.ID = 1;
					S.Subtitle.Label.text = string.Empty;
					S.Interaction = StudentInteractionType.Idle;
				}
				else if (S.ClubPhase == 2)
				{
					S.DialogueWheel.PracticeWindow.Club = S.Club;
					if (S.Club == ClubType.Gardening)
					{
						S.DialogueWheel.End();
					}
					else
					{
						S.DialogueWheel.PracticeWindow.FadeOut = true;
						S.DialogueWheel.PracticeWindow.FadeIn = false;
					}
					S.Subtitle.Label.text = string.Empty;
					S.Interaction = StudentInteractionType.Idle;
				}
				else if (S.ClubPhase == 3)
				{
					S.DialogueWheel.End();
				}
			}
		}
		else if (S.Interaction == StudentInteractionType.NamingCrush)
		{
			if (S.TalkTimer == 3f)
			{
				if (S.DialogueWheel.Victim != S.Crush)
				{
					S.Subtitle.UpdateLabel(SubtitleType.SuitorLove, 0, 3f);
					S.CharacterAnimation.CrossFade(S.GossipAnim);
					S.CurrentAnim = S.GossipAnim;
				}
				else
				{
					DatingGlobals.SuitorProgress = 1;
					S.Yandere.LoveManager.SuitorProgress++;
					S.Subtitle.UpdateLabel(SubtitleType.SuitorLove, 1, 3f);
					S.CharacterAnimation.CrossFade(S.Nod1Anim);
					S.CurrentAnim = S.Nod1Anim;
				}
			}
			else
			{
				if (Input.GetButtonDown(InputNames.Xbox_A))
				{
					S.TalkTimer = 0f;
				}
				if (S.CharacterAnimation[S.CurrentAnim].time >= S.CharacterAnimation[S.CurrentAnim].length)
				{
					S.CharacterAnimation.CrossFade(IdleAnim);
				}
				if (S.TalkTimer <= 0f)
				{
					S.DialogueWheel.End();
				}
			}
			S.TalkTimer -= Time.deltaTime;
		}
		else if (S.Interaction == StudentInteractionType.ChangingAppearance)
		{
			if (!S.StudentManager.DialogueWheel.AppearanceWindow.Show)
			{
				if (S.TalkTimer == 3f)
				{
					S.Subtitle.UpdateLabel(SubtitleType.SuitorLove, 2, 3f);
					S.CharacterAnimation.CrossFade(S.Nod1Anim);
				}
				else
				{
					if (Input.GetButtonDown(InputNames.Xbox_A))
					{
						S.TalkTimer = 0f;
					}
					if (S.CharacterAnimation[S.Nod1Anim].time >= S.CharacterAnimation[S.Nod1Anim].length)
					{
						S.CharacterAnimation.CrossFade(IdleAnim);
					}
					if (S.TalkTimer <= 0f)
					{
						Debug.Log("Apparently, " + base.name + "'s TalkTimer just reached 0.");
						S.DialogueWheel.End();
					}
				}
				S.TalkTimer -= Time.deltaTime;
			}
		}
		else if (S.Interaction == StudentInteractionType.Court)
		{
			if (S.TalkTimer == 3f)
			{
				bool flag10 = false;
				bool flag11 = false;
				if (S.CurrentAction == StudentActionType.SitAndEatBento)
				{
					flag10 = true;
				}
				if (S.StudentID == S.DialogueWheel.LoveManager.SuitorID)
				{
					S.Subtitle.UpdateLabel(SubtitleType.SuitorLove, 3, 5f);
					S.CharacterAnimation.CrossFade(S.Nod1Anim);
				}
				else if ((!S.Male && S.BikiniAttacher.enabled && !S.MyRenderer.enabled) || S.Schoolwear == 2)
				{
					S.Subtitle.CustomText = "Bad timing. As you can see, I'm in a swimsuit right now. Maybe later.";
					S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 10f);
					S.CharacterAnimation.CrossFade(S.GossipAnim);
					Refuse = true;
				}
				else if (S.HelpOffered)
				{
					S.Subtitle.UpdateLabel(SubtitleType.SuitorLove, 6, 5f);
					S.CharacterAnimation.CrossFade(S.GossipAnim);
					Refuse = true;
				}
				else if (flag11)
				{
					S.Subtitle.UpdateLabel(SubtitleType.StudentStay, 3, 13f);
					S.TalkTimer = 13f;
					Refuse = true;
				}
				else if (flag10)
				{
					S.Subtitle.CustomText = "Ugh...now? Seriously? I'm busy eating...can it wait?";
					S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
					S.TalkTimer = 5f;
					Refuse = true;
				}
				else if (!S.Indoors)
				{
					S.Subtitle.CustomText = "Sorry, it's a bit early in the day for that. At least wait until I've changed my shoes.";
					S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
					S.TalkTimer = 5f;
					Refuse = true;
				}
				else
				{
					S.Subtitle.UpdateLabel(SubtitleType.SuitorLove, 4, 5f);
					S.CharacterAnimation.CrossFade(S.Nod1Anim);
				}
			}
			else
			{
				if (Input.GetButtonDown(InputNames.Xbox_A))
				{
					S.TalkTimer = 0f;
				}
				if (S.CharacterAnimation[S.Nod1Anim].time >= S.CharacterAnimation[S.Nod1Anim].length)
				{
					S.CharacterAnimation.CrossFade(IdleAnim);
				}
				if (S.TalkTimer <= 0f)
				{
					if (!Refuse)
					{
						S.MeetTime = S.Clock.HourTime - 1f;
						if (S.StudentID == S.DialogueWheel.LoveManager.SuitorID)
						{
							S.MeetSpot = S.StudentManager.SuitorSpot;
						}
						else
						{
							S.MeetSpot = S.StudentManager.RomanceSpot;
							S.StudentManager.LoveManager.RivalWaiting = true;
						}
						S.Hurry = true;
						S.Pathfinding.speed = 4f;
						S.MeetTime = S.Clock.HourTime;
					}
					S.DialogueWheel.End();
					Refuse = false;
				}
			}
			S.TalkTimer -= Time.deltaTime;
		}
		else if (S.Interaction == StudentInteractionType.Advice)
		{
			if (S.TalkTimer == 5f)
			{
				S.Subtitle.UpdateLabel(SubtitleType.SuitorLove, 5, 99f);
				S.CharacterAnimation.CrossFade(S.Nod1Anim);
			}
			else
			{
				if (Input.GetButtonDown(InputNames.Xbox_A))
				{
					S.TalkTimer = 0f;
				}
				if (S.CharacterAnimation[S.Nod1Anim].time >= S.CharacterAnimation[S.Nod1Anim].length)
				{
					S.CharacterAnimation.CrossFade(IdleAnim);
				}
				if (S.TalkTimer <= 0f)
				{
					S.Rose = true;
					S.DialogueWheel.End();
				}
			}
			S.TalkTimer -= Time.deltaTime;
		}
		else if (S.Interaction == StudentInteractionType.Feeding)
		{
			Debug.Log("Feeding.");
			if (S.TalkTimer == 10f)
			{
				if (S.Club == ClubType.Delinquent)
				{
					S.CharacterAnimation.CrossFade(S.IdleAnim);
					S.Subtitle.UpdateLabel(SubtitleType.RejectFood, 1, 3f);
				}
				else if (S.Fed || S.Club == ClubType.Council || S.StudentID == 22)
				{
					S.CharacterAnimation.CrossFade(S.GossipAnim);
					S.Subtitle.UpdateLabel(SubtitleType.RejectFood, 0, 3f);
					S.Fed = true;
				}
				else
				{
					S.CharacterAnimation.CrossFade(S.Nod2Anim);
					if (S.Yandere.PickUp.AmaiTask)
					{
						S.Subtitle.UpdateLabel(SubtitleType.AcceptFood, 1, 3f);
					}
					else
					{
						S.Subtitle.UpdateLabel(SubtitleType.AcceptFood, 0, 3f);
					}
					CalculateRepBonus();
					S.Reputation.PendingRep += 1f + (float)S.RepBonus;
					S.PendingRep += 1f + (float)S.RepBonus;
				}
			}
			else if (Input.GetButtonDown(InputNames.Xbox_A))
			{
				S.TalkTimer = 0f;
			}
			if (S.CharacterAnimation[S.Nod2Anim].time >= S.CharacterAnimation[S.Nod2Anim].length)
			{
				S.CharacterAnimation.CrossFade(S.IdleAnim);
			}
			if (S.CharacterAnimation[S.GossipAnim].time >= S.CharacterAnimation[S.GossipAnim].length)
			{
				S.CharacterAnimation.CrossFade(S.IdleAnim);
			}
			S.TalkTimer -= Time.deltaTime;
			if (S.TalkTimer <= 0f)
			{
				if (!S.Fed && S.Club != ClubType.Delinquent)
				{
					S.Yandere.PickUp.FoodPieces[S.Yandere.PickUp.Food].SetActive(value: false);
					S.Yandere.PickUp.Food--;
					S.Fed = true;
				}
				S.DialogueWheel.End();
				S.StudentManager.UpdateStudents();
			}
		}
		else if (S.Interaction == StudentInteractionType.TaskInquiry)
		{
			Debug.Log(S.Name + " is currently being told to respond to a Task Inquiry.");
			if (S.TalkTimer == 10f)
			{
				if (S.Club == ClubType.Bully)
				{
					S.CharacterAnimation.CrossFade("f02_embar_00");
					S.Subtitle.UpdateLabel(SubtitleType.TaskInquiry, S.StudentID - 80, 10f);
				}
				else if (S.StudentID == 10)
				{
					S.CharacterAnimation.CrossFade("f02_nod_00");
					if (S.FollowTarget != null)
					{
						S.Subtitle.UpdateLabel(SubtitleType.TaskInquiry, 7, 10f);
					}
					else
					{
						S.Subtitle.UpdateLabel(SubtitleType.TaskInquiry, 8, 8f);
					}
				}
			}
			else if (Input.GetButtonDown(InputNames.Xbox_A))
			{
				S.TalkTimer = 0f;
			}
			if (S.CharacterAnimation["f02_embar_00"].time >= S.CharacterAnimation["f02_embar_00"].length)
			{
				S.CharacterAnimation.CrossFade(IdleAnim);
			}
			if (S.CharacterAnimation["f02_nod_00"].time >= S.CharacterAnimation["f02_nod_00"].length)
			{
				S.CharacterAnimation.CrossFade(IdleAnim);
			}
			S.TalkTimer -= Time.deltaTime;
			if (S.TalkTimer <= 0f)
			{
				if (S.Club == ClubType.Bully)
				{
					S.StudentManager.TaskManager.GirlsQuestioned[S.StudentID - 80] = true;
				}
				else if (S.StudentID == 10)
				{
					S.Destinations[S.Phase] = S.StudentManager.RaibaruMentorSpot;
					S.Pathfinding.target = S.StudentManager.RaibaruMentorSpot;
					S.CurrentDestination = S.StudentManager.RaibaruMentorSpot;
					S.Actions[S.Phase] = StudentActionType.Socializing;
					S.CurrentAction = StudentActionType.Socializing;
					if (S.FollowTarget != null)
					{
						S.FollowTarget.Follower = null;
					}
					S.StudentManager.TaskManager.Mentored = true;
					S.Pathfinding.speed = 4f;
					S.Mentoring = true;
					S.InEvent = true;
					S.Hurry = true;
				}
				S.DialogueWheel.End();
			}
		}
		else if (S.Interaction == StudentInteractionType.TakingSnack)
		{
			Debug.Log(S.Name + " is reacting to being offered a snack.");
			if (S.TalkTimer == 5f)
			{
				bool flag12 = false;
				if (S.StudentID == S.StudentManager.RivalID && !S.StudentManager.MissionMode)
				{
					if (S.StudentManager.Eighties)
					{
						if (S.StudentID == 14 || S.StudentID == 15 || S.StudentID == 19)
						{
							flag12 = true;
						}
						else if (S.Clock.Period > 2 && S.StudentManager.RivalBookBag.BentoStolen)
						{
							S.Hungry = true;
							S.Fed = false;
						}
						else
						{
							flag12 = true;
						}
					}
					else if (!S.Hungry)
					{
						Debug.Log("The rival is not hungry, so she is going to refuse the snack.");
						flag12 = true;
					}
					else
					{
						Debug.Log("Osana is hungry, and should accept the snack.");
					}
				}
				if (S.Club == ClubType.Delinquent && !S.StudentManager.MissionMode)
				{
					S.CharacterAnimation.CrossFade(S.IdleAnim);
					S.Subtitle.UpdateLabel(SubtitleType.RejectFood, 1, 3f);
					S.IgnoreFoodTimer = 10f;
				}
				else if (S.Fed || S.Club == ClubType.Council || flag12 || S.StudentID == 22)
				{
					S.CharacterAnimation.CrossFade(S.GossipAnim);
					S.Subtitle.UpdateLabel(SubtitleType.RejectFood, 0, 3f);
					S.IgnoreFoodTimer = 10f;
					S.Fed = true;
					if (S.StudentID == S.StudentManager.RivalID)
					{
						if (S.StudentManager.Eighties)
						{
							if (S.StudentID == 14)
							{
								S.Subtitle.CustomText = "No offense, but I don't eat junk food. Sorry!";
							}
							else if (S.StudentID == 15)
							{
								S.Subtitle.CustomText = "I refuse to put something that inexpensive in my mouth!";
							}
							else if (S.StudentID == 19)
							{
								S.Subtitle.CustomText = "I can't eat that - it would ruin my figure! Sorry!";
							}
							else
							{
								S.Subtitle.CustomText = "No thanks, I brought my own lunch to school today!";
							}
							S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
						}
						else
						{
							S.Subtitle.UpdateLabel(SubtitleType.RejectFood, 2, 5f);
						}
					}
					Debug.Log(S.Name + " has refused the snack.");
				}
				else
				{
					S.CharacterAnimation.CrossFade(S.Nod2Anim);
					S.Subtitle.UpdateLabel(SubtitleType.AcceptFood, 0, 10f);
					CalculateRepBonus();
					S.Reputation.PendingRep += 1f + (float)S.RepBonus;
					S.PendingRep += 1f + (float)S.RepBonus;
				}
			}
			else if (Input.GetButtonDown(InputNames.Xbox_A))
			{
				S.TalkTimer = 0f;
			}
			if (S.CharacterAnimation[S.Nod2Anim].time >= S.CharacterAnimation[S.Nod2Anim].length)
			{
				S.CharacterAnimation.CrossFade(S.IdleAnim);
			}
			if (S.CharacterAnimation[S.GossipAnim].time >= S.CharacterAnimation[S.GossipAnim].length)
			{
				S.CharacterAnimation.CrossFade(S.IdleAnim);
			}
			S.TalkTimer -= Time.deltaTime;
			if (S.TalkTimer <= 0f)
			{
				bool flag13 = false;
				if (S.Club == ClubType.Delinquent && !S.StudentManager.MissionMode)
				{
					flag13 = true;
				}
				if (!S.Fed && !flag13)
				{
					if (S.StudentID == S.StudentManager.RivalID && SchemeGlobals.GetSchemeStage(4) == 5)
					{
						SchemeGlobals.SetSchemeStage(4, 6);
						S.Yandere.PauseScreen.Schemes.UpdateInstructions();
					}
					PickUpScript pickUp = S.Yandere.PickUp;
					S.Yandere.EmptyHands();
					S.EmptyHands();
					pickUp.GetComponent<MeshFilter>().mesh = S.StudentManager.OpenChipBag;
					pickUp.transform.parent = S.LeftItemParent;
					pickUp.transform.localPosition = new Vector3(-0.02f, -0.075f, 0f);
					pickUp.transform.localEulerAngles = new Vector3(-15f, -15f, 30f);
					pickUp.MyRigidbody.useGravity = false;
					pickUp.MyRigidbody.isKinematic = true;
					pickUp.Prompt.Hide();
					pickUp.Prompt.enabled = false;
					pickUp.enabled = false;
					S.BagOfChips = pickUp.gameObject;
					S.EatingSnack = true;
					S.Private = true;
					S.Hungry = false;
					S.Fed = true;
				}
				S.DialogueWheel.End();
				S.StudentManager.UpdateStudents();
			}
		}
		else if (S.Interaction == StudentInteractionType.GivingHelp)
		{
			if (S.TalkTimer == 4f)
			{
				if (S.Club == ClubType.Council || S.Club == ClubType.Delinquent)
				{
					S.CharacterAnimation.CrossFade(S.GossipAnim);
					S.Subtitle.UpdateLabel(SubtitleType.RejectHelp, 0, 4f);
				}
				else if (S.Yandere.Bloodiness > 0f)
				{
					S.CharacterAnimation.CrossFade(S.GossipAnim);
					S.Subtitle.UpdateLabel(SubtitleType.RejectHelp, 1, 4f);
				}
				else
				{
					S.CharacterAnimation.CrossFade(S.PullBoxCutterAnim);
					S.SmartPhone.SetActive(value: false);
					S.Subtitle.UpdateLabel(SubtitleType.GiveHelp, 0, 4f);
				}
			}
			else
			{
				Input.GetButtonDown(InputNames.Xbox_A);
			}
			if (S.CharacterAnimation[S.GossipAnim].time >= S.CharacterAnimation[S.GossipAnim].length)
			{
				S.CharacterAnimation.CrossFade(S.IdleAnim);
			}
			if (S.CharacterAnimation[S.PullBoxCutterAnim].time >= S.CharacterAnimation[S.PullBoxCutterAnim].length)
			{
				S.CharacterAnimation.CrossFade(S.IdleAnim);
			}
			S.TalkTimer -= Time.deltaTime;
			if (S.Club != ClubType.Council && S.Club != ClubType.Delinquent)
			{
				S.MoveTowardsTarget(S.Yandere.transform.position + S.Yandere.transform.forward * 0.75f);
				if (S.CharacterAnimation[S.PullBoxCutterAnim].time >= S.CharacterAnimation[S.PullBoxCutterAnim].length)
				{
					S.CharacterAnimation.CrossFade(S.IdleAnim);
					StuckBoxCutter = null;
				}
				else if (S.CharacterAnimation[S.PullBoxCutterAnim].time >= 2f)
				{
					if (StuckBoxCutter.transform.parent != S.RightEye)
					{
						StuckBoxCutter.Prompt.enabled = true;
						StuckBoxCutter.enabled = true;
						StuckBoxCutter.transform.parent = S.Yandere.PickUp.transform;
						StuckBoxCutter.transform.localPosition = new Vector3(0f, 0.19f, 0f);
						StuckBoxCutter.transform.localEulerAngles = new Vector3(0f, -90f, 0f);
					}
				}
				else if (S.CharacterAnimation[S.PullBoxCutterAnim].time >= 1.166666f && StuckBoxCutter == null)
				{
					StuckBoxCutter = S.Yandere.PickUp.StuckBoxCutter;
					S.Yandere.PickUp.StuckBoxCutter = null;
					StuckBoxCutter.InBox = false;
					StuckBoxCutter.FingerprintID = S.StudentID;
					StuckBoxCutter.transform.parent = S.RightHand;
					StuckBoxCutter.transform.localPosition = new Vector3(0f, 0f, 0f);
					StuckBoxCutter.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
				}
			}
			if (S.TalkTimer <= 0f)
			{
				S.DialogueWheel.End();
				S.StudentManager.UpdateStudents();
			}
		}
		else if (S.Interaction == StudentInteractionType.SentToLocker)
		{
			bool flag14 = false;
			if (S.Club == ClubType.Delinquent && !S.StudentManager.MissionMode)
			{
				flag14 = true;
			}
			if (S.Friend)
			{
				flag14 = false;
			}
			if (S.TalkTimer == 5f)
			{
				if (!flag14)
				{
					Refuse = false;
					bool flag15 = true;
					if (S.Club == ClubType.Delinquent)
					{
						flag15 = true;
					}
					if ((!flag15 && S.Clock.HourTime > 8f && S.Clock.HourTime < 13f) || (flag15 && S.Clock.HourTime > 13.375f && S.Clock.HourTime < 15.5f) || S.Schoolwear == 2)
					{
						if (S.Schoolwear == 2)
						{
							S.CharacterAnimation.CrossFade(S.GossipAnim);
							S.Subtitle.CustomText = "Thanks for letting me know, but...I'm in a swimsuit right now. Remind me later.";
							S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
							Refuse = true;
						}
						else
						{
							S.CharacterAnimation.CrossFade(S.GossipAnim);
							S.Subtitle.UpdateLabel(SubtitleType.SendToLocker, 1, 5f);
							Refuse = true;
						}
					}
					else if (!S.StudentManager.MissionMode && S.Club == ClubType.Council)
					{
						S.CharacterAnimation.CrossFade(S.GossipAnim);
						S.Subtitle.UpdateLabel(SubtitleType.SendToLocker, 3, 5f);
						Refuse = true;
					}
					else if (!S.StudentManager.MissionMode && S.Rival)
					{
						if (!S.Friend || S.Reputation.Reputation < (float)(DateGlobals.Week * 10))
						{
							if (!S.Friend)
							{
								S.Yandere.NotificationManager.CustomText = "You must befriend her first";
								S.Yandere.NotificationManager.DisplayNotification(NotificationType.Custom);
							}
							if (S.Reputation.Reputation < (float)(DateGlobals.Week * 10))
							{
								S.Yandere.NotificationManager.CustomText = "You need at least " + DateGlobals.Week * 10 + " Reputation Points";
								S.Yandere.NotificationManager.DisplayNotification(NotificationType.Custom);
							}
							S.CharacterAnimation.CrossFade(S.GossipAnim);
							S.Subtitle.UpdateLabel(SubtitleType.SendToLocker, 4, 5f);
							Refuse = true;
						}
						else if (S.Rival && S.Clock.HourTime > 13f && S.Clock.HourTime < 13.375f)
						{
							S.CharacterAnimation.CrossFade(S.GossipAnim);
							S.Subtitle.CustomText = "Thanks for letting me know, but...I'm eating lunch right now. Remind me later.";
							S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
							Refuse = true;
						}
						else
						{
							S.CharacterAnimation.CrossFade(S.Nod1Anim);
							S.Subtitle.UpdateLabel(SubtitleType.SendToLocker, 2, 5f);
						}
					}
					else
					{
						S.CharacterAnimation.CrossFade(S.Nod1Anim);
						S.Subtitle.UpdateLabel(SubtitleType.SendToLocker, 2, 5f);
						if (S.StudentID == 41 && !S.StudentManager.Eighties)
						{
							S.Subtitle.CustomText = "Oh? Okay.";
							S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
						}
					}
				}
				else
				{
					S.Subtitle.UpdateLabel(SubtitleType.Dismissive, 5, 5f);
				}
			}
			else
			{
				if (Input.GetButtonDown(InputNames.Xbox_A))
				{
					S.TalkTimer = 0f;
				}
				if (S.CharacterAnimation[S.Nod1Anim].time >= S.CharacterAnimation[S.Nod1Anim].length)
				{
					S.CharacterAnimation.CrossFade(IdleAnim);
				}
				if (S.TalkTimer <= 0f)
				{
					if (!Refuse)
					{
						if (!flag14)
						{
							S.Pathfinding.speed = 4f;
							S.TargetDistance = 1f;
							S.SentToLocker = true;
							S.Routine = false;
							S.CanTalk = false;
						}
						else
						{
							S.Pathfinding.speed = 1f;
							S.TargetDistance = 0.5f;
							S.Routine = true;
							S.CanTalk = true;
						}
					}
					S.DialogueWheel.End();
				}
			}
			S.TalkTimer -= Time.deltaTime;
		}
		else if (S.Interaction == StudentInteractionType.WaitingForBeatEmUpResult)
		{
			Debug.Log("We are currently ''waiting for beat em up result''");
			if (!FadeIn)
			{
				S.Subtitle.Darkness.alpha = Mathf.MoveTowards(S.Subtitle.Darkness.alpha, 1f, Time.deltaTime);
				if (S.Subtitle.Darkness.alpha == 1f)
				{
					S.TriggerBeatEmUpMinigame();
					FadeIn = true;
				}
			}
			else
			{
				Debug.Log("''FadeIn'' is false.");
				S.Subtitle.Darkness.alpha = Mathf.MoveTowards(S.Subtitle.Darkness.alpha, 0f, Time.deltaTime);
				if (S.Subtitle.Darkness.alpha == 0f)
				{
					Debug.Log("''Darkness'' is 0.");
					if (!GameGlobals.BeatEmUpSuccess)
					{
						S.DialogueWheel.End();
						Timer = 0f;
					}
					else
					{
						Debug.Log("''BeatEmUpSuccess'' is true.");
						S.StudentManager.UpdateAllAnimLayers();
						S.Yandere.SetAnimationLayers();
						AstarPath.active.Scan();
						S.TaskPhase = 5;
						S.Interaction = StudentInteractionType.GivingTask;
						Timer = 0f;
						Debug.Log("Now telling " + S.Name + " to go home.");
						S.CurrentDestination.transform.position = S.StudentManager.Exit.position;
						ScheduleBlock obj = S.ScheduleBlocks[6];
						obj.destination = "Exit";
						obj.action = "Stand";
						S.GetDestinations();
					}
					FadeIn = false;
				}
			}
		}
		if (S.StudentID == 41 && !S.DialogueWheel.ClubLeader && S.Interaction != StudentInteractionType.ClubUnwelcome && S.Interaction != StudentInteractionType.GivingTask && !S.StudentManager.Eighties && S.TalkTimer > 0f)
		{
			Debug.Log("Geiju response.");
			if (S.Grudge)
			{
				S.Subtitle.CustomText = "Murderer. Begone.";
				S.Subtitle.UpdateLabel(SubtitleType.Custom, 0, 5f);
			}
			else if (NegativeResponse)
			{
				Debug.Log("Negative response.");
				S.Subtitle.UpdateLabel(SubtitleType.Impatience, 6, 5f);
			}
			else
			{
				S.Subtitle.UpdateLabel(SubtitleType.Impatience, 5, 5f);
			}
		}
		if (S.Waiting)
		{
			S.WaitTimer -= Time.deltaTime;
			if (!(S.WaitTimer <= 0f))
			{
				return;
			}
			S.DialogueWheel.TaskManager.UpdateTaskStatus();
			S.Talking = false;
			S.Waiting = false;
			base.enabled = false;
			if (!Fake && !S.StudentManager.CombatMinigame.Practice)
			{
				S.Pathfinding.canSearch = true;
				S.Pathfinding.canMove = true;
				S.Obstacle.enabled = false;
				S.Alarmed = false;
				if (!S.Following && !S.Distracting && !S.Wet && !S.EatingSnack && !S.SentToLocker)
				{
					S.Routine = true;
				}
				if (!S.Following)
				{
					ParticleSystem.EmissionModule emission = S.Hearts.emission;
					emission.enabled = false;
				}
			}
			S.StudentManager.EnablePrompts();
			if (S.GoAway)
			{
				Debug.Log("This student was just told to go away.");
				S.CurrentDestination = S.StudentManager.GoAwaySpots.List[S.StudentID];
				S.Pathfinding.target = S.StudentManager.GoAwaySpots.List[S.StudentID];
				S.Pathfinding.canSearch = true;
				S.Pathfinding.canMove = true;
				S.DistanceToDestination = 100f;
			}
		}
		else
		{
			S.targetRotation = Quaternion.LookRotation(new Vector3(S.Yandere.transform.position.x, base.transform.position.y, S.Yandere.transform.position.z) - base.transform.position);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, S.targetRotation, 10f * Time.deltaTime);
		}
	}

	private void CheckForGossipSpecialCase()
	{
		Debug.Log("Checking for gossip speacial case.");
		RejectGossip = false;
		if (!Eighties)
		{
			if ((S.StudentID == 2 && S.DialogueWheel.Victim == 3) || (S.StudentID == 3 && S.DialogueWheel.Victim == 2) || (S.StudentID == 10 && S.DialogueWheel.Victim == 11) || (S.StudentID == 11 && S.DialogueWheel.Victim == 10) || (S.StudentID == 25 && S.DialogueWheel.Victim == 30) || (S.StudentID == 30 && S.DialogueWheel.Victim == 25) || (S.OriginalClub == ClubType.MartialArts && S.DialogueWheel.Victim == 10))
			{
				if (S.OriginalClub == ClubType.MartialArts && S.DialogueWheel.Victim == 10)
				{
					RejectGossipLine = "Hey! I have a lot of respect for her! Don't say anything weird about her!";
				}
				else if (S.DialogueWheel.Victim < 4)
				{
					RejectGossipLine = "Hey! She's my sister! Don't say anything weird about her!";
				}
				else
				{
					RejectGossipLine = "Hey! She's my friend! Don't say anything weird about her!";
				}
				RejectGossip = true;
			}
		}
		else if (S.StudentID > 1 && S.StudentID < 6 && S.DialogueWheel.Victim < 6)
		{
			RejectGossipLine = "Hey! She's my friend! Don't say anything weird about her!";
			RejectGossip = true;
		}
		else if (S.StudentID > 5 && S.StudentID < 11 && S.DialogueWheel.Victim < 11)
		{
			RejectGossipLine = "Hey! He's my friend! Don't say anything weird about him!";
			RejectGossip = true;
		}
		if (S.Club != 0 && S.Club == S.StudentManager.Students[S.DialogueWheel.Victim].Club)
		{
			if (S.StudentManager.Students[S.DialogueWheel.Victim].Male)
			{
				RejectGossipLine = "Hey! He's my clubmate! Don't say anything weird about him!";
			}
			else
			{
				RejectGossipLine = "Hey! She's my clubmate! Don't say anything weird about her!";
			}
			RejectGossip = true;
		}
		if (S.InCouple && S.DialogueWheel.Victim == S.PartnerID)
		{
			if (S.StudentManager.Students[S.DialogueWheel.Victim].Male)
			{
				RejectGossipLine = "Hey! He's my boyfriend! Don't say anything weird about him!";
			}
			else
			{
				RejectGossipLine = "Hey! She's my girlfriend! Don't say anything weird about her!";
			}
			RejectGossip = true;
		}
		for (int i = 1; i < 11; i++)
		{
			if (S.StudentID == S.StudentManager.SuitorIDs[i])
			{
				Debug.Log("This guy's ID is in the suitor list.");
				if (S.DialogueWheel.Victim == S.Crush)
				{
					RejectGossipLine = "Hey! I don't appreciate you saying things like that about her!";
					RejectGossip = true;
				}
			}
		}
	}

	public void CalculateRepBonus()
	{
		S.RepBonus = 0;
		if (PlayerGlobals.PantiesEquipped == 3)
		{
			S.RepBonus++;
		}
		if ((S.Male && S.Yandere.Class.Seduction + S.Yandere.Class.SeductionBonus > 0) || S.Yandere.Class.Seduction == 5)
		{
			S.RepBonus++;
		}
		if (S.Yandere.Class.SocialBonus > 0)
		{
			S.RepBonus++;
		}
		S.ChameleonCheck();
		if (S.Chameleon)
		{
			S.RepBonus++;
		}
		S.RepBonus += S.Yandere.Class.PsychologyGrade + S.Yandere.Class.PsychologyBonus;
		Debug.Log("''RepBonus'' is: " + S.RepBonus);
		Debug.Log("Reputation will go up by " + (S.RepBonus + 1) + " and Friendship will go up by " + (S.RepBonus + 1));
		S.DialogueWheel.Social.Student = S;
		S.DialogueWheel.Social.StudentID = S.StudentID;
		S.DialogueWheel.Social.StudentFriendships[S.StudentID] += 1 + S.RepBonus;
		S.DialogueWheel.Social.CheckFriendStatus();
		S.DialogueWheel.Social.UpdateButtons();
	}

	private void LateUpdate()
	{
		if (!S.Talking || !(S.Yandere.TalkTimer <= 0f) || (!S.Male && S.Club == ClubType.Delinquent))
		{
			return;
		}
		if (S.Subtitle.CurrentClip != null && S.Subtitle.Speaker == S)
		{
			if (AudioData.MyAudioSource == null)
			{
				AudioData.MyAudioSource = S.Subtitle.CurrentClip.GetComponent<AudioSource>();
			}
		}
		else
		{
			AudioData.MyAudioSource = null;
		}
		if (AudioData.MyAudioSource != null)
		{
			S.Jaw.localEulerAngles += new Vector3(0f, 0f, AudioData.g[1].transform.position.y);
			if (S.Jaw.localEulerAngles.z < 40f)
			{
				S.Jaw.localEulerAngles = new Vector3(S.Jaw.localEulerAngles.x, S.Jaw.localEulerAngles.y, 40f);
			}
			else if (S.Jaw.localEulerAngles.z > 55f)
			{
				S.Jaw.localEulerAngles = new Vector3(S.Jaw.localEulerAngles.x, S.Jaw.localEulerAngles.y, 55f);
			}
		}
	}
}
