using LSNoir.Callouts.SA.Commons;
using LSNoir.Callouts.SA.Data;
using LSPD_First_Response.Mod.API;
using Rage;
using System.Linq;
using LSNoir.Extensions;
using static LtFlash.Common.Serialization.Serializer;

namespace LSNoir.Callouts.SA.Creators
{
    public static class InterrogationCreator
    {
        public static InterrogationLine[] InterrogationLineCreator(Type type, Ped perp)
        {
            string player = Settings.OfficerName();
            string pedName = Functions.GetPersonaForPed(perp).FullName;
            string pedMrMrs;
            if (perp.IsMale)
                pedMrMrs = "Mr. " + Functions.GetPersonaForPed(perp).Surname;
            else
                pedMrMrs = "Ms. " + Functions.GetPersonaForPed(perp).Surname;

            var _cData = LoadItemFromXML<CaseData>(Main.CDataPath);
            var _vData = GetSelectedListElementFromXml<PedData>(Main.PDataPath,
                c => c.FirstOrDefault(v => v.Type == PedType.Victim));
            var _sData = GetSelectedListElementFromXml<PedData>(Main.SDataPath,
                c => c.FirstOrDefault(s => s.Type == PedType.Suspect));

            InterrogationData data = new InterrogationData {Ped = perp};
            data.IncreaseAsked(3);

            switch (type)
            {
                case Type.VictimFamily:
                    InterrogationLine[] question1, question2, question3;
                
                    data.InterrogationType = InterrogationData.Type.VictimFamily;

                    QuestionType q;

                    // QUESTION 1 //
                    switch (MathHelper.GetRandomInteger(2))
                    {
                        case 0:
                            data.QuestionsAsked.Add(InterrogationData.Questions.Vic1);
                            q = QuestionType.Relationships;

                            question1 = new InterrogationLine[]
                            {
                                // Question 1 -- Significant other: Who did they know?
                                new InterrogationLine(InterrogationLine.Type.Question,
                                    "[~p~" + player + "~w~] Hello, " + pedMrMrs + ", I'm Detective " + player +
                                    " and I have a few questions I need you to answer. First, do you know anyone who might have wanted to hurt the victim, " +
                                    _vData.Name,
                                    "[~y~" + pedName +
                                    "~w~] Um, oh. I don't really know them too well, they probably knew some people.",
                                    InterrogationLine.Type.Doubt),
                                new InterrogationLine(InterrogationLine.Type.Truth,
                                    "[~p~" + player +
                                    "~w~] Okay, thank you for that answer. I have two more questions for you.",
                                    "[~y~" + pedName +
                                    "~w~] Certainly. I think that they were dating someone. I can't remember their name though."),
                                new InterrogationLine(InterrogationLine.Type.Doubt,
                                    "[~p~" + player +
                                    "~w~] Come on, you must know something more. Who else do they know?",
                                    "[~y~" + pedName +
                                    "~w~] Let me think for a minute... Um... They were dating someone, I can't think of their name though, it's escaping me."),
                                new InterrogationLine(InterrogationLine.Type.Lie,
                                    "[~p~" + player +
                                    "~w~] I know you're lying to me, okay. Tell me the truth or I'll have to take you downtown.",
                                    "[~y~" + pedName +
                                    "~w~] I just had a traumatizing experience and you're accusing me of lying?! Maybe I should ask for a new detective. They were dating someone a while ago, that's all I remember."),
                            };
                            q.ToString().AddLog();
                            break;
                        case 1:
                            data.QuestionsAsked.Add(InterrogationData.Questions.Vic2);
                            q = QuestionType.LastKnownActivity;
                            question1 = new InterrogationLine[]
                            {
                                // Question 1 -- Friends: Last known activity
                                new InterrogationLine(InterrogationLine.Type.Question,
                                    "[~p~" + player + "~w~] Hello, " + pedMrMrs + ", my name is Detective " + player +
                                    " and I have a few questions I need you to answer. First, do you know what the last thing, " +
                                    _vData.Name + " was doing?",
                                    "[~y~" + pedName +
                                    "~w~] I know they were hanging out with a friend, I believe. I think I saw it on their LifeInvader profile.",
                                    InterrogationLine.Type.Truth),
                                new InterrogationLine(InterrogationLine.Type.Truth,
                                    "[~p~" + player +
                                    "~w~] Alright, thank you. I have two more questions to ask, if you don't mind.",
                                    "[~y~" + pedName +
                                    "~w~] Certainly. If I remember correctly I saw their relationship status changed."),
                                new InterrogationLine(InterrogationLine.Type.Doubt,
                                    "[~p~" + player + "~w~] Yeah, okay, well you must know something more than that.",
                                    "[~y~" + pedName +
                                    "~w~] I don't like your tone, detective. Really- that's it. The only other thing is I saw they had changed their relationship status to 'in a relationship'."),
                                new InterrogationLine(InterrogationLine.Type.Lie,
                                    "[~p~" + player + "~w~] Don't make me take you downtown, okay?",
                                    "[~y~" + pedName +
                                    "~w~] Hey, hey! I'm the one who has to deal with this! Don't you dare tell me I'm lying! The only other thing that might be important is that they started dating someone, according to their LifeInvader profile."),
                            };
                            q.ToString().AddLog();
                            break;
                        default:
                            data.QuestionsAsked.Add(InterrogationData.Questions.Vic3);
                            q = QuestionType.AnythingStrange;
                            question1 = new InterrogationLine[]
                            {
                                // Question 1 -- Strange activity: Anything strange?
                                new InterrogationLine(InterrogationLine.Type.Question,
                                    "[~p~" + player + "~w~] How's it going, " + pedMrMrs + "? My name is Detective " +
                                    player +
                                    " and I have a couple questions that I need you to answer. Have you noticed " +
                                    _vData.Name + " doing anything abnormal lately?",
                                    "[~y~" + pedName +
                                    "~w~] No, from what I remember they do the same things they've been doing for a while. Why?",
                                    InterrogationLine.Type.Truth),
                                new InterrogationLine(InterrogationLine.Type.Truth,
                                    "[~p~" + player + "~w~] Ah, okay. Just trying to get all the facts, " + pedMrMrs +
                                    ". I have two more questions I need you to answer",
                                    "[~y~" + pedName + "~w~] Sure, I have time."),
                                new InterrogationLine(InterrogationLine.Type.Doubt,
                                    "[~p~" + player + "~w~] Yeah, okay. What have they been doing recently?",
                                    "[~y~" + pedName +
                                    "~w~] Um... They've really only been doing normal, everyday- working, hanging out with friends, etc."),
                                new InterrogationLine(InterrogationLine.Type.Lie,
                                    "[~p~" + player +
                                    "~w~] Hey- look at me. I can't catch this person with you lying to me. What have they been doing recently?!",
                                    "[~y~" + pedName +
                                    "~w~] Are you really accusing me of lying?! Some detective you are. They haven't been doing anything out of the ordinary, okay? Next question."),
                            };
                            q.ToString().AddLog();
                            break;
                    }

                    // QUESTION 2 //
                    switch (q)
                    {
                        case QuestionType.Relationships:
                        q.ToString().AddLog();
                            question2 = new InterrogationLine[]
                            {
                                // Question 2 -- Significant other: name
                                new InterrogationLine(InterrogationLine.Type.Question,
                                    "[~p~" + player + "~w~] Okay, thank you. Can you please try to think of their name?",
                                    "[~y~" + pedName + "~w~] Um, I really don't think I can remember it.",
                                    InterrogationLine.Type.Doubt),
                                new InterrogationLine(InterrogationLine.Type.Truth,
                                    "[~p~" + player +
                                    "~w~] Okay, thank you for that answer. I have one more questions for you.",
                                    "[~y~" + pedName +
                                    $"~w~] Sure, shoot. I actually think they're name can be found on their LifeInvader profile. I think it was {_sData.Name}."),
                                new InterrogationLine(InterrogationLine.Type.Doubt,
                                    "[~p~" + player +
                                    "~w~] Come on, you must know something more. Who else do they know?",
                                    "[~y~" + pedName +
                                    "~w~] Let me think for a minute... Um... They were dating someone, I can't think of their name though, it's escaping me."),
                                new InterrogationLine(InterrogationLine.Type.Lie,
                                    "[~p~" + player +
                                    "~w~] Look, I really need their name. I can't do my job without it.",
                                    "[~y~" + pedName +
                                    "~w~] Did you really just tell me that you need their name?  You really think I wouldn't give it to you if I had it? How dare you!"),
                            };
                            break;
                        case QuestionType.LastKnownActivity:
                            question2 = new InterrogationLine[]
                            {
                                // Question 2 -- Friends: name
                                new InterrogationLine(InterrogationLine.Type.Question,
                                    "[~p~" + player +
                                    "~w~] Okay, do you remember what the person's name was? Perhaps you can show me their profile?",
                                    "[~y~" + pedName +
                                    "~w~] Um, I'm not too sure. I just glanced at their profile and I don't have my phone on me, I'm sorry.",
                                    InterrogationLine.Type.Truth),
                                new InterrogationLine(InterrogationLine.Type.Truth,
                                    "[~p~" + player +
                                    "~w~] Alright, thank you, I can check into it. I do have one more question for you, though.",
                                    "[~y~" + pedName + "~w~] No problem. Alright, shoot- well, not literally."),
                                new InterrogationLine(InterrogationLine.Type.Doubt,
                                    "[~p~" + player +
                                    "~w~] I really need you to help me out here. You have to know their name, please?",
                                    "[~y~" + pedName +
                                    "~w~] I really don't know, detective. I can't tell you something I don't know."),
                                new InterrogationLine(InterrogationLine.Type.Lie,
                                    "[~p~" + player +
                                    "~w~] You've got to know their name, come on. I can't do my job unless you tell me their name.",
                                    "[~y~" + pedName +
                                    "~w~] I always thought detectives were smart and resourceful; but apparently they're just lazy and forceful. I really don't know their name, detective."),
                            };
                            break;
                        default:
                            question2 = new InterrogationLine[]
                            {
                                // Question 2 -- Strange activity: Friendly with new people
                                new InterrogationLine(InterrogationLine.Type.Question,
                                    "[~p~" + player + "~w~] Hm, okay. Have they been friendly with anyone new lately?",
                                    "[~y~" + pedName + "~w~] Um, I don't know. That's their personal life, not mine.",
                                    InterrogationLine.Type.Lie),
                                new InterrogationLine(InterrogationLine.Type.Truth,
                                    "[~p~" + player +
                                    "~w~] Okay, thank you for that answer. I have two more questions for you.",
                                    "[~y~" + pedName +
                                    "~w~] Certainly. I think that they were dating someone. I can't remember their name though."),
                                new InterrogationLine(InterrogationLine.Type.Doubt,
                                    "[~p~" + player +
                                    "~w~] Come on, you must know something more. Who else do they know?",
                                    "[~y~" + pedName +
                                    "~w~] Let me think for a minute... Um... They were dating someone, I can't think of their name though, it's escaping me."),
                                new InterrogationLine(InterrogationLine.Type.Lie,
                                    "[~p~" + player +
                                    "~w~] Hey - you need to be truthful with me here if we're going to catch who did this. Every detail helps, even the first letter of a name.",
                                    "[~y~" + pedName +
                                    "~w~] Ugh I really don't know, the only thing I can remember is their name started with " +
                                    _sData.Name[0] + " " + _sData.Name[1] + " " + _sData.Name[2])
                            };
                            break;
                    }

                    // QUESTION 3 //
                    switch (q)
                    {
                        case QuestionType.Relationships:
                            question3 = new InterrogationLine[]
                            {
                                // Question 3 -- Significant other: Last place seen
                                new InterrogationLine(InterrogationLine.Type.Question,
                                    "[~p~" + player +
                                    "~w~] Alright, well, can you think of the last place you saw them?",
                                    "[~y~" + pedName +
                                    "~w~] Uh, I remember they went out somewhere together. I can't recall where though.",
                                    InterrogationLine.Type.Truth),
                                new InterrogationLine(InterrogationLine.Type.Truth,
                                    "[~p~" + player +
                                    "~w~] Alright, that's fine; I will check their social media to see if they posted it lately. Thank you very much for answering all my questions.",
                                    "[~y~" + pedName + "~w~] Thank you for your help, detective."),
                                new InterrogationLine(InterrogationLine.Type.Doubt,
                                    "[~p~" + player +
                                    "~w~] Are you sure that you can't recall? It would really help me narrow down my investigation.",
                                    "[~y~" + pedName +
                                    "~w~] I honestly don't know, officer. I have no reason to withhold information from you. Please do catch this person."),
                                new InterrogationLine(InterrogationLine.Type.Lie,
                                    "[~p~" + player +
                                    "~w~] Look, you have to know where they went. You weren't snooping on them or anything?",
                                    "[~y~" + pedName +
                                    "~w~] You really think I would eavesdrop on them?! How dare you! I hope you catch the person, but I can't believe you'd say that."),
                            };
                            break;
                        case QuestionType.AnythingStrange:
                            question3 = new InterrogationLine[]
                            {
                                // Question 3 -- Friends: last time hung out
                                new InterrogationLine(InterrogationLine.Type.Question,
                                    "[~p~" + player +
                                    "~w~] Alright, my last question: do you know the last time they might have seen each other?",
                                    "[~y~" + pedName + "~w~] Ummm... I don't recall officer.",
                                    InterrogationLine.Type.Doubt),
                                new InterrogationLine(InterrogationLine.Type.Truth,
                                    "[~p~" + player +
                                    "~w~] Okay, good to know. I'll check into it myself. Thank you for answering my questions.",
                                    "[~y~" + pedName +
                                    "~w~] Of course. I do think they hung out recently though, now that I think about it. Please get this person, detective."),
                                new InterrogationLine(InterrogationLine.Type.Doubt,
                                    "[~p~" + player +
                                    "~w~] You really need to work with me here. Please try to think harder.",
                                    "[~y~" + pedName +
                                    "~w~] Alright, alright... Actually- I believe they were together recently. But I'm not 100% sure. Please do your best to get this person, detective."),
                                new InterrogationLine(InterrogationLine.Type.Lie,
                                    "[~p~" + player + "~w~] You have to know when they were last together!",
                                    "[~y~" + pedName +
                                    "~w~] If I knew, I would tell you! Now stop telling me I'm a liar! Good day, detective."),
                            };
                            break;
                        default:
                            question3 = new InterrogationLine[]
                            {
                                // Question 3 -- Strange activity: Last time seen them
                                new InterrogationLine(InterrogationLine.Type.Question,
                                    "[~p~" + player +
                                    "~w~] Okay, can you try to tell me the last time you saw them together? Or the last time you knew they were meeting up?",
                                    "[~y~" + pedName +
                                    "~w~] If I recall correctly, it was recently. But do make sure I'm right, detective. Thank you for your help.",
                                    InterrogationLine.Type.Truth),
                                new InterrogationLine(InterrogationLine.Type.Truth,
                                    "[~p~" + player + "~w~] Great to know, I'll check it out. Thank you.",
                                    "[~y~" + pedName +
                                    "~w~] Great; thank you for everything, detective. Please try to get that terrible person."),
                                new InterrogationLine(InterrogationLine.Type.Doubt,
                                    "[~p~" + player +
                                    "~w~] Please let me know everything you know, okay? I need all the information I can to solve this case.",
                                    "[~y~" + pedName +
                                    "~w~] I told you everything I know; I don't know exactly when, but it was recently. Please get whoever did this."),
                                new InterrogationLine(InterrogationLine.Type.Lie,
                                    "[~p~" + player +
                                    "~w~] I really need you to give me all the information you can. Please don't hold out on me.",
                                    "[~y~" + pedName +
                                    "~w~] You really think I'm not telling you everything? How dare you! Just do me a favor and don't talk to me unti you get the person that did this."),
                            };
                            break;
                    }

                    InterrogationLine[] final = question1.Concat(question2).ToArray().Concat(question3).ToArray();

                    return final;
                case Type.Suspect:
                    pedMrMrs.ToString().AddLog();
                    _cData.CrimeTime.ToShortDateString().AddLog();
                    _cData.CrimeTime.ToShortTimeString().AddLog();
                    _sData.Name.ToString().AddLog();

                    var q1 = new InterrogationLine[]
                    {
                        // Question 1 -- Where were you on this day?
                        new InterrogationLine(InterrogationLine.Type.Question,
                            "[~p~" + player + "~w~] Hello, " + pedMrMrs + ", I'm Detective " + player +
                            " and I have a few questions I need you to answer. Where were you on " +
                            _cData.CrimeTime.ToShortDateString() + " at about " + _cData.CrimeTime.ToShortTimeString(),
                            "[~y~" + _sData.Name +
                            "~w~] Um, I don't think I need to answer your questions. But, since I have nothing to hide I was hanging out with friends.",
                            InterrogationLine.Type.Doubt),
                        new InterrogationLine(InterrogationLine.Type.Truth,
                            "[~p~" + player + "~w~] Alright; I'm assuming they can prove your alibi? I'll talk to them.",
                            "[~y~" + _sData.Name +
                            "~w~] Of course, probably. I mean, we got drunk so maybe I don't really remember where I was."),
                        new InterrogationLine(InterrogationLine.Type.Doubt,
                            "[~p~" + player + "~w~] I know that's a joke. Where were you?",
                            "[~y~" + _sData.Name + "~w~] Alright, well, I was only with one other person."),
                        new InterrogationLine(InterrogationLine.Type.Lie,
                            "[~p~" + player +
                            "~w~] I know you're lying to me, okay. Tell me the truth or I'll have to take you downtown.",
                            "[~y~" + _sData.Name + "~w~] Woah, woah; relax pig! I was with one person, relax."),
                    };
                    var q2 = new InterrogationLine[]
                    {
                        // Question 2 -- Do you know this person?
                        new InterrogationLine(InterrogationLine.Type.Question,
                            "[~p~" + player + "~w~] Alright - now that we've got that straight, how do you know " +
                            _vData.Name + "?",
                            "[~y~" + _sData.Name + "~w~] Um, I don't know them, sorry.",
                            InterrogationLine.Type.Lie),
                        new InterrogationLine(InterrogationLine.Type.Truth,
                            "[~p~" + player + "~w~] Oh, you don't? Okay. Next question.",
                            "[~y~" + _sData.Name +
                            "~w~] Well actually, I might be dating them, but that's not illegal. Am I done answering questions now?"),
                        new InterrogationLine(InterrogationLine.Type.Doubt,
                            "[~p~" + player + "~w~] Come on, we all know you know them. How?",
                            "[~y~" + _sData.Name +
                            "~w~] Okay - look, we were, umm... 'hooking up', okay. I'm not proud of it but it's whatever. Am I done answering questions now?"),
                        new InterrogationLine(InterrogationLine.Type.Lie,
                            "[~p~" + player +
                            "~w~] Yeah, bullshit. Tell me how you know them now, or I'll be sure to bring you downtown and show you the real LSPD way!",
                            "[~y~" + _sData.Name +
                            "~w~] Hey, hey, okay whatever. We're uh... 'dating' now, alright. We're keeping it low-key though so don't tell anyone. Am I done answering questions now?"),
                    };
                    var q3 = new InterrogationLine[]
                    {
                        // Question 3 -- When did you last see this person?
                        new InterrogationLine(InterrogationLine.Type.Question,
                            "[~p~" + player + "~w~] I have one more question for you - when did you last see " +
                            _vData.Name + "?",
                            "[~y~" + _sData.Name + "~w~] I haven't seen them in a while, alright?",
                            InterrogationLine.Type.Lie),
                        new InterrogationLine(InterrogationLine.Type.Truth,
                            "[~p~" + player +
                            "~w~] Okay, I'll believe that. I will be coming back for you though - okay?.",
                            "[~y~" + _sData.Name +
                            "~w~] Hey, relax with the threats po-po. And I prob saw them recently, too. I'll be right here waiting for you, don't worry."),
                        new InterrogationLine(InterrogationLine.Type.Doubt,
                            "[~p~" + player +
                            "~w~] This isn't my first case dealing with assholes. Tell me what I want to know.",
                            "[~y~" + _sData.Name + "~w~] Alright, alright. I saw them around the time you asked me earlier."),
                        new InterrogationLine(InterrogationLine.Type.Lie,
                            "[~p~" + player +
                            "~w~] Hah, what do you take me for, an idiot? Tell me the truth or I'll have to break out my nightstick",
                            "[~y~" + _sData.Name +
                            "~w~] I have rights!! But I'll play your game - I might have been around then around the time you asked about. But that's all your getting out of me."),
                    };
                    var f = q1.Concat(q2).ToArray().Concat(q3).ToArray();

                    return f;
                default:
                    return null;
            }
        }

        public enum Type { VictimFamily, Suspect }

        public enum QuestionType { Relationships, LastKnownActivity, AnythingStrange }
    }
}
