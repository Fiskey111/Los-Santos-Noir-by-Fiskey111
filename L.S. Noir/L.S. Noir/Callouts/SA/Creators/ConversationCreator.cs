using LSNoir.Callouts.SA.Commons;
using LSNoir.Callouts.SA.Data;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using Rage;
using System.Collections.Generic;
using Fiskey111Common;
using LSNoir.Callouts.SA.Stages.ME;
using LSNoir.Callouts.Universal;
using LSNoir.Extensions;

namespace LSNoir.Callouts.SA.Creators
{
    class ConversationCreator
    {
        public static List<string> Text = new List<string>();

        public static DialogLine[] DialogLineCreator(ConversationType type, Ped ped, int witnumber = 0, bool victimAlive = false, PedData vicdata = null, PedData susdata = null, PedData witdata = null)
        {
            string playerName = Settings.OfficerName();
            List<string> value = new List<string>();
            string fullName = Functions.GetPersonaForPed(ped).FullName;

            if (type == ConversationType.Fo)
            {
                string lastname = Functions.GetPersonaForPed(ped).Surname;

                value.Add("[~p~" + playerName + "~w~] What do we have here, " + lastname + "?");
                value.Add("[~p~" + playerName + "~w~] Ugh, I can't stand these calls, " + lastname + "?");
                value.Add("[~p~" + playerName + "~w~] Wow.  This scene looks crazy, " + lastname + "?");
                value.Add("[~p~" + playerName + "~w~] How's everything going, " + lastname + "?");

                DialogLine line1 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();

                value.Add("[~b~Officer " + lastname + "~w~] So I arrived on scene and found them here like this.");
                value.Add("[~b~Officer " + lastname + "~w~] Looks to me like a sexual assault. Take a look at the body and see for yourself.");
                value.Add("[~b~Officer " + lastname + "~w~] Yeah, it's a sexual assault. Check the victim's body and see what you notice.");
                value.Add("[~b~Officer " + lastname + "~w~] I can't stand these calls.  Hopefully one day they'll stop.");

                DialogLine line2 = new DialogLine(1, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();

                value.Add("[~p~" + playerName + "~w~] Damn.  Do we have any ~o~witnesses~w~?");
                value.Add("[~p~" + playerName + "~w~] Well, hopefully we have some witnesses. Do we?");
                value.Add("[~p~" + playerName + "~w~] Anyone witness anything around here?");
                value.Add("[~p~" + playerName + "~w~] Please tell me we have some witnesses.");

                DialogLine line3 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();

                if (witnumber == 0)
                {
                    value.Add("[~b~Officer " + lastname + "~w~] I wish I could say otherwise, but no.");
                    value.Add("[~b~Officer " + lastname + "~w~] I know that would help you but there aren't any nearby.");
                    value.Add("[~b~Officer " + lastname + "~w~] I didn't come upon anyone when I arrived, so no.");
                    value.Add("[~b~Officer " + lastname + "~w~] Nope, no witnesses at all, sorry.");
                }
                else
                {
                    value.Add("[~b~Officer " + lastname + "~w~] Yes!  We actually have " + witnumber.ToString() + " go talk to them near my cruiser.");
                    value.Add("[~b~Officer " + lastname + "~w~] Thank goodness I arrived quickly, we have " + witnumber.ToString() + " standing near my cruiser.");
                    value.Add("[~b~Officer " + lastname + "~w~] " + witnumber.ToString() + " of them were found; their next to my cruiser");
                    value.Add("[~b~Officer " + lastname + "~w~] Actually we got " + witnumber.ToString() + "! Go to my car to see them.");
                }

                DialogLine line4 = new DialogLine(1, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();

                value.Add("[~p~" + playerName + "~w~] Anything else that I need to know?");
                value.Add("[~p~" + playerName + "~w~] Alright, thanks.  Anything else you need to tell me?");
                value.Add("[~p~" + playerName + "~w~] Okay.  What else is important to know?");
                value.Add("[~p~" + playerName + "~w~] Hm, great.  Is there anything else important?");

                DialogLine line5 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();

                value.Add("[~b~Officer " + lastname + "~w~] I called EMS, they're on their way over now.");
                value.Add("[~b~Officer " + lastname + "~w~] Yeah, I called EMS. They're responding now.");
                value.Add("[~b~Officer " + lastname + "~w~] I actually just called EMS, they're heading over now.");
                value.Add("[~b~Officer " + lastname + "~w~] Dispatch just sent over EMS, so be ready for them.");

                DialogLine line6 = new DialogLine(1, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();

                value.Add("[~p~" + playerName + "~w~] Alright, thanks for your help.  I'll take it from here.");
                value.Add("[~p~" + playerName + "~w~] Thanks; hopefully I can figure this one out.");
                value.Add("[~p~" + playerName + "~w~] Thanks for all your help; hopefully I can get this solved soon.");
                value.Add("[~p~" + playerName + "~w~] Well, this looks difficult.  I'll contact you if I need you at all.");

                DialogLine line7 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();
                
                DialogLine[] d1 =
                    {
                        line1, line2, line3, line4, line5, line6, line7
                    };
                
                return d1;
            }
            else if (type == ConversationType.Ems)
            {
                value.Add("[~p~" + playerName + "~w~] How's the victim doing?");
                value.Add("[~p~" + playerName + "~w~] What's the news with the victim?");

                DialogLine line1 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();

                DialogLine line2, line3, line4;

                if (victimAlive)
                {
                    value.Add("[~g~Paramedic~w~] They're alive, albeit barely.");
                    value.Add("[~g~Paramedic~w~] They seem to be okay, we'll know more later.");
                    value.Add("[~g~Paramedic~w~] Based on my expertise, they'll survive.");

                    line2 = new DialogLine(1, value[Rand.RandomNumber(0, value.Count)]);
                    value.Clear();

                    value.Add("[~p~" + playerName + "~w~] Fantastic! When can I speak to them?");
                    value.Add("[~p~" + playerName + "~w~] Great! That's awesome!  How can I contact them?");
                    value.Add("[~p~" + playerName + "~w~] Perfect; What's their next step?");

                    line3 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                    value.Clear();

                    value.Add("[~g~Paramedic~w~] We're going to take them to the hospital. You can speak to their physician later.");
                    value.Add("[~g~Paramedic~w~] They'll be transported to the hospital now; come later to speak to their physician to learn more.");
                    value.Add("[~g~Paramedic~w~] The patient will be brought to the nearest hospital.  Stop off later and talk to their physician.");

                    line4 = new DialogLine(1, value[Rand.RandomNumber(0, value.Count)]);
                    value.Clear();
                }
                else
                {
                    value.Add("[~g~Paramedic~w~] Sadly they're dead; there was nothing else we could do.");
                    value.Add("[~g~Paramedic~w~] DOA; I hate arriving to a dead victim.");
                    value.Add("[~g~Paramedic~w~] From what I can tell they've been dead for a bit, sorry.");

                    line2 = new DialogLine(1, value[Rand.RandomNumber(0, value.Count)]);
                    value.Clear();

                    value.Add("[~p~" + playerName + "~w~] Damn. How can I get a report about their death?");
                    value.Add("[~p~" + playerName + "~w~] Hm, that sucks. What can I do to get information on their death?");
                    value.Add("[~p~" + playerName + "~w~] Alright. What's the next step?");

                    line3 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                    value.Clear();

                    value.Add("[~g~Paramedic~w~] You'll have to call the coroner and let them deal with it, sorry.");
                    value.Add("[~g~Paramedic~w~] Call a coroner; they'll take care of the body.");
                    value.Add("[~g~Paramedic~w~] Go call the coroner; I'm not too sure what happens after that.");

                    line4 = new DialogLine(1, value[Rand.RandomNumber(0, value.Count)]);
                    value.Clear();
                }

                value.Add("[~p~" + playerName + "~w~] Alright, thanks. I'll continue investigating the scene.");
                value.Add("[~p~" + playerName + "~w~] Great! I'll work on the case in the meantime.");
                value.Add("[~p~" + playerName + "~w~] Okay; that works. Looks like I'll hang around here and continue the case.");

                DialogLine line5 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();

                DialogLine[] d1 =
                    {
                        line1, line2, line3, line4, line5
                    };

                return d1;
            }
            else if (type == ConversationType.Coroner)
            {
                value.Add("[~p~" + playerName + "~w~] How's it going, buddy? What can you tell me?");
                value.Add("[~p~" + playerName + "~w~] Howdy; what's your thoughts on the victim?");

                DialogLine line1 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();

                value.Add("[~g~Coroner~w~] Hm. I can't tell much right now, but I'll be able to let you know more later.");
                value.Add("[~g~Coroner~w~] We'll know more later once we get him back to the ME office in Los Santos.");
                value.Add("[~g~Coroner~w~] Not too sure. Looks bad; not the worst I've seen though. We'll know more later.");

                DialogLine line2 = new DialogLine(1, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();

                value.Add("[~p~" + playerName + "~w~] Alright, not the best news but I'll take it.");
                value.Add("[~p~" + playerName + "~w~] Okay - not what I wanted to hear but I'll stop off soon.");
                value.Add("[~p~" + playerName + "~w~] I figured as much, I'll wait to contact the ME.");

                DialogLine line3 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();

                DialogLine[] d1 =
                    {
                        line1, line2, line3
                    };

                return d1;
            }
            else if (type == ConversationType.Witness)
            {
                value.Add("[~p~" + playerName + "~w~] Hello, I'm Detective " + playerName + ".  I have a couple questions for you.");
                value.Add("[~p~" + playerName + "~w~] Hello, my name is Detective " + playerName + " and I have a couple questions for you.");

                DialogLine line1 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();

                value.Add("[~o~" + fullName + "~w~] Certainly, anything to help!");
                value.Add("[~o~" + fullName + "~w~] Um, sure, I guess. I don't have anything new to say.");
                value.Add("[~o~" + fullName + "~w~] I suppose I can help, although I don't really want to.");

                DialogLine line2 = new DialogLine(1, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();

                value.Add("[~p~" + playerName + "~w~] Alright - what did you see here today?.");
                value.Add("[~p~" + playerName + "~w~] Did you see anything happen near here?");
                value.Add("[~p~" + playerName + "~w~] Can you tell me what you saw?");

                DialogLine line3 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();

                DialogLine line4, line5, line6, line7;

                if (witdata != null)
                {
                    value.Add("[~o~" + fullName + "~w~] I saw " + witdata.WhatSeenString + " here a while ago.  Not sure if that helps.");
                    value.Add("[~o~" + fullName + "~w~] Hm.  Umm... I think I saw " + witdata.WhatSeenString + " right in this alley a short time before you arrived.");
                    value.Add("[~o~" + fullName + "~w~] Uhhh I believe " + witdata.WhatSeenString + " was near here in this alleyway a short while ago.");

                    line4 = new DialogLine(1, value[Rand.RandomNumber(0, value.Count)]);
                    value.Clear();

                    value.Add("[~p~" + playerName + "~w~] Okay, good to know. Anything else?");
                    value.Add("[~p~" + playerName + "~w~] Thanks! What else did you see here?");
                    value.Add("[~p~" + playerName + "~w~] Did you see anything important around?.");

                    line5 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                    value.Clear();

                    value.Add("[~o~" + fullName + "~w~] No, sorry, that's all I saw.");
                    value.Add("[~o~" + fullName + "~w~] Nope, that's it I think.");
                    value.Add("[~o~" + fullName + "~w~] I can't think of anything else.");

                    line6 = new DialogLine(1, value[Rand.RandomNumber(0, value.Count)]);
                    value.Clear();

                    value.Add("[~p~" + playerName + "~w~] Darn, okay. If you think of anything please give me a call. Here's my card.");
                    value.Add("[~p~" + playerName + "~w~] Hm, okay. If that changes please get in touch with me. Here's my card.");
                    value.Add("[~p~" + playerName + "~w~] Alright, well, please think about it and talk to me if anything changes. Here's my card.");

                    line7 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                    value.Clear();

                    DialogLine[] d1 =
                    {
                        line1, line2, line3, line4, line5, line6, line7
                    };

                    return d1;
                }
                else
                {
                    value.Add("[~o~" + fullName + "~w~] I wish I could help but I didn't see anything.");
                    value.Add("[~o~" + fullName + "~w~] Ummm... I don't think I saw anything here except the body.");
                    value.Add("[~o~" + fullName + "~w~] I only called the police about the body I found, that's it.");

                    line4 = new DialogLine(1, value[Rand.RandomNumber(0, value.Count)]);
                    value.Clear();

                    value.Add("[~p~" + playerName + "~w~] Darn, okay. If you think of anything please give me a call. Here's my card.");
                    value.Add("[~p~" + playerName + "~w~] Hm, okay. If that changes please get in touch with me. Here's my card.");
                    value.Add("[~p~" + playerName + "~w~] Alright, well, please think about it and talk to me if anything changes. Here's my card.");

                    line5 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                    value.Clear();

                    DialogLine[] d1 =
                    {
                        line1, line2, line3, line4, line5
                    };

                    return d1;
                }
            }
            else if (type == ConversationType.Physician)
            {
                bool important = vicdata.IsImportant;
                bool dna = false;
                string result = null;
                Traces traces = vicdata.Traces;
                int noMatch, match, incorrectMatch, total, chance, percent;
                string lastname = Functions.GetPersonaForPed(ped).Surname;
                if (important)
                {
                    if (traces == (Traces.Blood | Traces.Semen | Traces.Hair | Traces.Saliva | Traces.Wearer | Traces.Urine))
                    {
                        //DNA   no match = value, match = value, incorrect match = value
                        //Overall probability = value/total
                        dna = true;
                        noMatch = 65; match = 30; incorrectMatch = 5;
                        total = noMatch + match + incorrectMatch;
                        chance = Rand.RandomNumber(0, 101);
                        percent = chance / total;
                        if (percent >= 65)
                        { result = "~y~DNA evidence~w~ returned no match with any suspects."; }
                        else if (percent < 65 && percent >= 30)
                        { result = "~y~DNA evidence~w~ returned a match with a suspect named ~r~" + susdata.Name; }
                        else
                        {
                            string fullname = Persona.GetRandomFullName();
                            if (fullname == susdata.Name)
                                fullname = Persona.GetRandomFullName();
                            if (fullname == susdata.Name)
                                fullname = Persona.GetRandomFullName();
                            result = "~y~DNA evidence~w~ returned a match with a suspect named ~r~" + fullname;
                        }
                        result.AddLog();
                    }
                    else if (traces == Traces.Fingerprint)
                    {
                        noMatch = 45; match = 40; incorrectMatch = 15;
                        total = noMatch + match + incorrectMatch;
                        chance = Rand.RandomNumber(0, 101);
                        percent = chance / total;
                        if (percent >= 45)
                        { result = "~y~fingerprint evidence~w~ returned no match with any suspects."; }
                        else if (percent < 45 && percent >= 15)
                        { result = "~y~fingerprint evidence~w~ returned a match with a suspect named ~r~" + susdata.Name; }
                        else
                        {
                            string fullname = Persona.GetRandomFullName();
                            if (fullname == susdata.Name)
                                fullname = Persona.GetRandomFullName();
                            if (fullname == susdata.Name)
                                fullname = Persona.GetRandomFullName();
                            result = "~y~fingerprint evidence~w~ returned a match with a suspect named ~r~" + fullname;
                        }
                        result.AddLog();
                    }
                    else if (traces == Traces.Weapon)
                    {
                        noMatch = 40; match = 45; incorrectMatch = 15;
                        total = noMatch + match + incorrectMatch;
                        chance = Rand.RandomNumber(0, 101);
                        percent = chance / total;
                        if (percent < 40 && percent >= 15)
                        { result = "~y~weapon evidence~w~ returned no match with any suspects."; }
                        else if (percent > 40)
                        { result = "~y~weapon evidence~w~ returned a match with a suspect named ~r~" + susdata.Name; }
                        else
                        {
                            string fullname = Persona.GetRandomFullName();
                            if (fullname == susdata.Name)
                                fullname = Persona.GetRandomFullName();
                            if (fullname == susdata.Name)
                                fullname = Persona.GetRandomFullName();
                            result = "~y~weapon evidence~w~ returned a match with a suspect named ~r~" + fullname;
                        }
                        result.AddLog();
                    }
                    else
                        result = "Umm.. I don't remember, sorry!";
                }

                value.Add("[~p~" + playerName + "~w~] Hello, I'm Detective " + playerName + ". What sort of information do you have for me regarding " + vicdata.Name + "?");
                value.Add("[~p~" + playerName + "~w~] Hello, my name is Detective " + playerName + "; what updates do you have on the victim, " + vicdata.Name + "?");

                DialogLine line1 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();

                DialogLine line2, line3;
                if (important)
                {
                    value.Add("[~g~" + fullName + "~w~] Oh, yes, of course. Let me think for a minute... They're alive, barely. Right, the " + result);
                    value.Add("[~g~" + fullName + "~w~] Ah yes, that was a difficult patient. We're lucky they survived. I believe the " + result);
                    value.Add("[~g~" + fullName + "~w~] Hmm... Let me think, it's been a long shift... " + vicdata.Name + "... Oh, right! The " + result);

                    line2 = new DialogLine(1, value[Rand.RandomNumber(0, value.Count)]);
                    value.Clear();

                    value.Add("[~p~" + playerName + "~w~] Damn, okay. I'll have to look somewhere else then. Thank you, Dr. " + lastname + ".");
                    value.Add("[~p~" + playerName + "~w~] Alright Dr. " + lastname + ". I think I can get a suspect another way. Thank you.");

                    line3 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                    value.Clear();
                }
                else
                {
                    value.Add("[~g~" + fullName + "~w~] Ummm... I've had so many patients let me think. Right, " + vicdata.Name + ". We didn't find anything at all, sorry.");
                    value.Add("[~g~" + fullName + "~w~] Oh right, they barely survived I remember. We actually didn't find anything, sorry about that!");
                    value.Add("[~g~" + fullName + "~w~] Yes, I remember. There wasn't anything we could find from them. I hope you have something else you can use to catch the suspect.");

                    line2 = new DialogLine(1, value[Rand.RandomNumber(0, value.Count)]);
                    value.Clear();

                    value.Add("[~p~" + playerName + "~w~] Damn, okay. I'll have to look somewhere else then. Thank you, Dr. " + lastname + ".");
                    value.Add("[~p~" + playerName + "~w~] Alright Dr. " + lastname + ". I think I can get a suspect another way. Thank you.");

                    line3 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                    value.Clear();
                }

                DialogLine[] d1 =
                    {
                        line1, line2, line3
                    };

                return d1;
            }
            else if (type == ConversationType.MeDriver)
            {
                value.Add("[~p~" + playerName + "~w~] Hello, I'm Detective " + playerName + ".  I was told to meet you here to go to the ME's office?");
                value.Add("[~p~" + playerName + "~w~] Hello, my name is Detective " + playerName + " and I was informed to meet you here so I can get a ride to the ME's office?");

                DialogLine line1 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();

                value.Add("[~o~" + fullName + "~w~] Ah, yes! Nice to meet you, you can either come with me or you can press ~y~E~w~ to drive yourself!");
                value.Add("[~o~" + fullName + "~w~] Oh, nice to meet you detective. You can come with me if you wish, otherwise you can press ~y~E~w~ to drive yourself there!");

                DialogLine line2 = new DialogLine(1, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();

                value.Add("[~o~" + fullName + "~w~] We have a patrol unit stationed nearby that will ensure your car is safe. The ME is in Los Santos, just so you know.");
                value.Add("[~o~" + fullName + "~w~] There is a patrol unit that will be stopping by to ensure your car is safe. In case you didn't know, the ME is in Central Los Santos.");

                DialogLine line3 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();

                value.Add("[~p~" + playerName + "~w~] Okay, thank you very much!");
                value.Add("[~p~" + playerName + "~w~] Alright, thank you!");

                DialogLine line4 = new DialogLine(1, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();

                DialogLine[] d1 =
                    {
                        line1, line2, line3, line4
                    };

                return d1;
            }
            else if (type == ConversationType.MedicalExaminer)
            {
                bool important = vicdata.IsImportant;
                bool dna = false;
                string result = null;
                Traces traces = vicdata.Traces;
                int noMatch, match, incorrectMatch, total, chance, percent;
                string lastname = Functions.GetPersonaForPed(ped).Surname;
                var _cData = LtFlash.Common.Serialization.Serializer.LoadItemFromXML<CaseData>(Main.CDataPath);
                if (important)
                {
                    switch (traces)
                    {
                        case (Traces.Blood | Traces.Semen | Traces.Hair | Traces.Saliva | Traces.Wearer | Traces.Urine):
                            //DNA   no match = value, match = value, incorrect match = value
                            //Overall probability = value/total
                            dna = true;
                            noMatch = 65; match = 30; incorrectMatch = 5;
                            total = noMatch + match + incorrectMatch;
                            chance = Rand.RandomNumber(0, 101);
                            percent = chance / total;
                            if (percent >= 65)
                            { result = "~y~DNA evidence~w~ returned no match with any suspects."; }
                            else if (percent < 65 && percent >= 30)
                            {
                                Sa_2BMedicalExaminer._isImportant = true;
                                result = "~y~DNA evidence~w~ returned a match with a suspect named ~r~" + susdata.Name; }
                            else
                            {
                                string fullname = Persona.GetRandomFullName();
                                if (fullname == susdata.Name)
                                    fullname = Persona.GetRandomFullName();
                                if (fullname == susdata.Name)
                                    fullname = Persona.GetRandomFullName();
                                Sa_2BMedicalExaminer._isImportant = true;
                                result = "~y~DNA evidence~w~ returned a match with a suspect named ~r~" + fullname;
                            }
                            break;
                        case Traces.Fingerprint:
                            noMatch = 45; match = 40; incorrectMatch = 15;
                            total = noMatch + match + incorrectMatch;
                            chance = Rand.RandomNumber(0, 101);
                            percent = chance / total;
                            if (percent >= 45)
                            { result = "~y~fingerprint evidence~w~ returned no match with any suspects."; }
                            else if (percent < 45 && percent >= 15)
                            {
                                Sa_2BMedicalExaminer._isImportant = true;
                                result = "~y~fingerprint evidence~w~ returned a match with a suspect named ~r~" + susdata.Name; }
                            else
                            {
                                string fullname = Persona.GetRandomFullName();
                                if (fullname == susdata.Name)
                                    fullname = Persona.GetRandomFullName();
                                if (fullname == susdata.Name)
                                    fullname = Persona.GetRandomFullName();
                                Sa_2BMedicalExaminer._isImportant = true;
                                result = "~y~fingerprint evidence~w~ returned a match with a suspect named ~r~" + fullname;
                            }
                            break;
                        default:
                            if (traces == Traces.Weapon)
                            {
                                noMatch = 40; match = 45; incorrectMatch = 15;
                                total = noMatch + match + incorrectMatch;
                                chance = Rand.RandomNumber(0, 101);
                                percent = chance / total;
                                if (percent < 40 && percent >= 15)
                                { result = "~y~weapon evidence~w~ returned no match with any suspects."; }
                                else if (percent > 40)
                                {
                                    Sa_2BMedicalExaminer._isImportant = true;
                                    result = "~y~weapon evidence~w~ returned a match with a suspect named ~r~" + susdata.Name;
                                }
                                else
                                {
                                    string fullname = Persona.GetRandomFullName();
                                    if (fullname == susdata.Name)
                                        fullname = Persona.GetRandomFullName();
                                    if (fullname == susdata.Name)
                                        fullname = Persona.GetRandomFullName();
                                    Sa_2BMedicalExaminer._isImportant = true;
                                    result = "~y~weapon evidence~w~ returned a match with a suspect named ~r~" + fullname;
                                }
                            }
                            else
                                result = "body turned up nothing at all, sadly.";
                            break;
                    }
                }

                value.Add("[~p~" + playerName + "~w~] Hello, I'm Detective " + playerName + ". What sort of information do you have for me regarding " + vicdata.Name + "?");
                value.Add("[~p~" + playerName + "~w~] Hello, my name is Detective " + playerName + "; what updates do you have on the victim, " + vicdata.Name + "?");

                DialogLine line1 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();

                DialogLine line2, line3, line4;

                if (important)
                {
                    value.Add("[~g~" + fullName + "~w~] Oh, yes, of course. Let me think for a minute... They were killed by strangulation. Right, the " + result);
                    value.Add("[~g~" + fullName + "~w~] Ah yes, that was a difficult case. It looks like they were killed by a broken neck. I believe the " + result);
                    value.Add("[~g~" + fullName + "~w~] Hmm... Let me think, it's been a long day here... " + vicdata.Name + "... Oh, right! The " + result);

                    line2 = new DialogLine(1, value[Rand.RandomNumber(0, value.Count)]);
                    value.Clear();

                    value.Add("[~g~" + fullName + $"~w~] My best estimate for the time of the attack was {_cData.CrimeTime.ToShortTimeString()}.");

                    line3 = new DialogLine(1, value[Rand.RandomNumber(0, value.Count)]);
                    value.Clear();

                    value.Add("[~p~" + playerName + "~w~] Great to know! I'll use that to help. Thank you, Dr. " + lastname+ ".");
                    value.Add("[~p~" + playerName + "~w~] Alright Dr. " + lastname + ". That hopefully will help!");

                    line4 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                    value.Clear();
                }
                else
                {
                    var cod = Rand.RandomNumber(0, 1) == 1 ? "Strangulation" : "Asphyxiation";

                    value.Add("[~g~" + fullName + "~w~] Ummm... I've had so many cases today let me think. Right, " + vicdata.Name + $". We found their cause of death as {cod}, sorry.");
                    value.Add("[~g~" + fullName + $"~w~] Oh right, they were a terrible case. We actually didn't find anything besided their cause of death, {cod} sorry about that!");
                    value.Add("[~g~" + fullName + $"~w~] Yes, I remember. You probably want to know their cause of death which was {cod}. I hope you have something else you can use to catch the suspect, though.");

                    line2 = new DialogLine(1, value[Rand.RandomNumber(0, value.Count)]);
                    value.Clear();

                    value.Add("[~g~" + fullName + $"~w~] My best estimate for the time of the attack was {_cData.CrimeTime.ToShortTimeString()}.");

                    line3 = new DialogLine(1, value[Rand.RandomNumber(0, value.Count)]);
                    value.Clear();

                    value.Add("[~p~" + playerName + "~w~] Damn, okay. I'll have to look somewhere else then. Thank you, Dr. " + lastname + ".");
                    value.Add("[~p~" + playerName + "~w~] Alright Dr. " + lastname + ". I'll try to get a name some other way.");

                    line4 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                    value.Clear();
                }

                DialogLine[] d1 =
                    {
                        line1, line2, line3, line4
                    };

                return d1;
            }
            if (type == ConversationType.VictimFamily)
            {
                string pronoun;
                if (ped.IsMale)
                    pronoun = "Mr. ";
                else
                    pronoun = "Ms. ";
                string lastname = Functions.GetPersonaForPed(ped).Surname;

                value.Add("[~p~" + playerName + "~w~] How's everything going, " + lastname + "?");
                value.Add("[~p~" + playerName + "~w~] Are you doing okay, " + lastname + "?");
                value.Add("[~p~" + playerName + "~w~] I hope you all are doing okay, " + lastname + "?");

                DialogLine line1 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();

                value.Add("[~y~" + pronoun + lastname + "~w~] I wish it didn't happen. I don't understand who would do this.");
                value.Add("[~y~" + pronoun + lastname + "~w~] We can't believe this happened. It still hasn't set in yet.");
                value.Add("[~y~" + pronoun + lastname + "~w~] I just don't know what to think anymore; I don't understand why this happened.");

                DialogLine line2 = new DialogLine(1, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();

                value.Add("[~p~" + playerName + "~w~] I will do everything in my power to bring whoever did this to justice. I just have a few questions for you.");
                value.Add("[~p~" + playerName + "~w~] We are going to work as hard as we can to get this person. I have a couple questions for you, if you don't mind.");
                value.Add("[~p~" + playerName + "~w~] I'll try my best to get this suspect, I promise. I have some questions I'd like to ask you.");

                DialogLine line3 = new DialogLine(0, value[Rand.RandomNumber(0, value.Count)]);
                value.Clear();

                DialogLine[] d1 =
                    {
                        line1, line2, line3
                    };

                return d1;
            }
            return null;
        }

        public enum ConversationType { Fo, Witness, Ems, Coroner, Physician, MeDriver, MedicalExaminer, VictimFamily, Suspect }
    }
}
