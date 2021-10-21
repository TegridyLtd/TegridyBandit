/////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2021 Tegridy Ltd                                          //
// Author: Darren Braviner                                                 //
// Contact: db@tegridygames.co.uk                                          //
/////////////////////////////////////////////////////////////////////////////
//                                                                         //
// This program is free software; you can redistribute it and/or modify    //
// it under the terms of the GNU General Public License as published by    //
// the Free Software Foundation; either version 2 of the License, or       //
// (at your option) any later version.                                     //
//                                                                         //
// This program is distributed in the hope that it will be useful,         //
// but WITHOUT ANY WARRANTY.                                               //
//                                                                         //
/////////////////////////////////////////////////////////////////////////////
//                                                                         //
// You should have received a copy of the GNU General Public License       //
// along with this program; if not, write to the Free Software             //
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,              //
// MA 02110-1301 USA                                                       //
//                                                                         //
/////////////////////////////////////////////////////////////////////////////

using System.Collections;
using UnityEngine;
using Tegridy.AudioTools;
using TMPro;
namespace Tegridy.Bandit
{
    public class TegridyBanditDigital : MonoBehaviour
    {
        public float playerCash;

        [Header("Controller Settings")]
        public GUIMachine[] machines;

        [Header("Default Audio Settings")]
        public AudioClip[] spinClick;
        public AudioClip[] spinSound;
        public AudioClip[] stopSound;
        public AudioClip[] winSound;
        public AudioClip[] stakeUpSound;
        public AudioClip[] stakeDownSound;

        AudioSource audioSource;

        [HideInInspector] public bool started = false;
        void Start()
        {
            //setup the machines
            for (int i = 0; i < machines.Length; i++)
            {
                machines[i].controller = new TegridyBanditController();
                machines[i].controller.bandit = new Bandit();

                //check for audio and set to default if none is found
                if (machines[i].gui.spinClick == null) machines[i].gui.spinClick = spinClick;
                if (machines[i].gui.spinSound == null) machines[i].gui.spinSound = spinSound;
                if (machines[i].gui.stopSound == null) machines[i].gui.stopSound = stopSound;
                if (machines[i].gui.winSound == null) machines[i].gui.winSound = winSound;
                if (machines[i].gui.stakeDownSound == null) machines[i].gui.stakeDownSound = stakeDownSound;
                if (machines[i].gui.stakeUpSound == null) machines[i].gui.stakeUpSound = stakeUpSound;

                //check settings make sense
                if (machines[i].gui.stakeMax < machines[i].gui.stakeIncrement) machines[i].gui.stakeMax = machines[i].gui.stakeIncrement;
                if (machines[i].gui.maxImageSwaps < machines[i].gui.minImageSwaps) machines[i].gui.maxImageSwaps = machines[i].gui.minImageSwaps;

                //add the listeners
                int thisOne = i;
                machines[thisOne].gui.spin.GetComponentInChildren<TextMeshProUGUI>().text = TegridyBanditLanguage.spin;
                machines[thisOne].gui.spin.onClick.AddListener(() => SpinWheel(thisOne));
                machines[thisOne].gui.stakeUp.onClick.AddListener(() => ChangeStake(thisOne, true));
                machines[thisOne].gui.stakeDown.onClick.AddListener(() => ChangeStake(thisOne, false));
                
                //setup the prizes
                machines[i].controller.bandit.houseRake = machines[i].gui.houseRake;
                machines[i].controller.bandit.winChance = machines[i].gui.winChance;
                machines[i].controller.bandit.prizes = new Prize[machines[i].gui.prizeSettings.Length];
                for (int i2 = 0; i2 < machines[i].controller.bandit.prizes.Length; i2++)
                {
                    machines[i].controller.bandit.prizes[i2] = new Prize();
                    machines[i].controller.bandit.prizes[i2] = machines[i].gui.prizeSettings[i2].prize;
                }

                //setup the wheels
                Wheel newWheel = new Wheel();
                newWheel.symbol = new Slot[machines[i].gui.prizeSettings.Length];
                for (int i2 = 0; i2 < newWheel.symbol.Length; i2++)
                {
                    newWheel.symbol[i2] = new Slot();
                    newWheel.symbol[i2].id = i2;
                }

                Wheel[] newWheels = new Wheel[machines[i].gui.wheels.Length];
                for (int i2 = 0; i2 < newWheels.Length; i2++)
                {
                    newWheels[i2] = newWheel;
                }
                machines[i].controller.bandit.wheels = newWheels;

                //finish up
                machines[i].currentPic = new int[newWheels.Length];
                machines[i].controller.BanditConfig(machines[i].controller.bandit);
                UpdateGUI(thisOne);
                ChangeStake(thisOne, false);
            }
            started = true;
        }
        public BanditResults SpinWheel(int machine)
        {
            if (machines[machine].currentStake <= playerCash && !machines[machine].spinning)
            {
                StopAllCoroutines();
                playerCash -= machines[machine].currentStake;
                BanditResults results = machines[machine].controller.SpinWheels(machines[machine].currentStake);
                machines[machine].spinning = true;
                UpdateGUI(machine);
                StartCoroutine(Spin(machine, results));
                return results;
            }
            else return null;
        }
        IEnumerator Spin(int machine, BanditResults results)
        {
            //play the click noise and setup some variables;
            TegridyAudioTools.PlayOneShot(machines[machine].gui.spinClick, audioSource);
            int swap = Random.Range(machines[machine].gui.minImageSwaps, machines[machine].gui.maxImageSwaps);
            int count = 0;
            bool[] stopped = new bool[results.wheelPositions.Length];

            //spin the wheels for a random amount of time
            while(count < swap)
            {
                for(int i = 0; i < machines[machine].gui.wheels.Length; i++)
                {
                    TegridyAudioTools.PlayOneShot(machines[machine].gui.spinSound, audioSource);
                    machines[machine].currentPic[i]++;
                    if (machines[machine].currentPic[i] >= machines[machine].gui.prizeSettings.Length) machines[machine].currentPic[i] = 0;
                    machines[machine].gui.wheels[i].sprite = machines[machine].gui.prizeSettings[machines[machine].currentPic[i]].picture;
                }
                count++;
                yield return new WaitForSeconds(machines[machine].gui.swapDelay);
            }
            //move to our results
            bool complete = false;
            while (!complete)
            {
                complete = true;
                for (int i = 0; i < machines[machine].gui.wheels.Length; i++)
                {
                    if (machines[machine].currentPic[i] != machines[machine].controller.bandit.wheels[i].symbol[results.wheelPositions[i]].id)
                    {
                        TegridyAudioTools.PlayOneShot(machines[machine].gui.spinSound, audioSource);
                        machines[machine].currentPic[i]++;
                        if (machines[machine].currentPic[i] >= machines[machine].gui.prizeSettings.Length) machines[machine].currentPic[i] = 0;
                        machines[machine].gui.wheels[i].sprite = machines[machine].gui.prizeSettings[machines[machine].currentPic[i]].picture;
                        complete = false;
                    }
                    else if (!stopped[i])
                    {
                        TegridyAudioTools.PlayOneShot(machines[machine].gui.stopSound, audioSource);
                        stopped[i] = true;
                    }
                }
                yield return new WaitForSeconds(machines[machine].gui.swapDelay);
            }

            //check if we have custom audio for the prize if not play the default
            if (results.winner)
            {
                if (machines[machine].gui.prizeSettings[results.prizeID].prize.prizeSound != null)
                {
                    TegridyAudioTools.PlayOneShot
                        (machines[machine].gui.prizeSettings[results.prizeID].prize.prizeSound, audioSource);
                }
                else TegridyAudioTools.PlayOneShot(machines[machine].gui.winSound, audioSource);
            }

            //update the display and add something to the player variable
            UpdateGUI(machine);
            playerCash += results.winnings;
       
            //wait till we can make another spin
            yield return new WaitForSeconds(machines[machine].gui.spinDelay);
            if (complete) machines[machine].spinning = false;

        }
        private void ChangeStake(int machine, bool up) 
        {
            if (up) 
            { 
                machines[machine].currentStake += machines[machine].gui.stakeIncrement;
                TegridyAudioTools.PlayOneShot(machines[machine].gui.stakeUpSound, audioSource);
            }
            else 
            {
                machines[machine].currentStake -= machines[machine].gui.stakeIncrement;
                TegridyAudioTools.PlayOneShot(machines[machine].gui.stakeDownSound, audioSource);
            }


            if (machines[machine].currentStake > machines[machine].gui.stakeMax) 
                machines[machine].currentStake = machines[machine].gui.stakeMax;
            else if (machines[machine].currentStake < machines[machine].gui.stakeIncrement) 
                machines[machine].currentStake = machines[machine].gui.stakeIncrement;
            machines[machine].gui.stake.text = machines[machine].currentStake.ToString("C");
        }
        private void UpdateGUI(int machine)
        {
            if(machines[machine].gui.cash != null)
            {
                machines[machine].gui.cash.text = "Cash " + playerCash.ToString("C");
            }

            if (machines[machine].gui.info != null)
            {
                Bandit bandit = machines[machine].controller.bandit;
                string stats = TegridyBanditLanguage.houseTake + (bandit.houseRake * 100).ToString() + "%";
                stats += "<br>" + TegridyBanditLanguage.winChance + (bandit.winChance * 100).ToString() + "%<br>";
                stats += "<br>" + TegridyBanditLanguage.totalTake + bandit.totalTake.ToString("F1");
                stats += "<br>" + TegridyBanditLanguage.totalPayout + bandit.totalPayOut.ToString("F1");
                stats += "<br>" + TegridyBanditLanguage.totalHousePot + bandit.housePot.ToString("F1");
                stats += "<br>" + TegridyBanditLanguage.totalSpins + bandit.totalSpins.ToString();
                stats += "<br>" + TegridyBanditLanguage.totalWin + bandit.totalWins.ToString();
                stats += "<br>" + TegridyBanditLanguage.totalLoses + bandit.totalLoses.ToString();
                stats += "<br><br>" + TegridyBanditLanguage.prizePotsTitle + "</b>";
                for (int i = 0; i < bandit.prizePots.Length; i++)
                {
                    stats += "<br>" + TegridyBanditLanguage.prizePot + i + " total = " + bandit.prizePots[i].ToString("F1");
                }
                machines[machine].gui.info.text = stats;
            }
        }
    }
}
