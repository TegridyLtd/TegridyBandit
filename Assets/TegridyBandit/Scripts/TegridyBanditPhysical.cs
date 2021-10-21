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
namespace Tegridy.Bandit
{
    public class TegridyBanditPhysical : MonoBehaviour
    {
        [Header("World Settings")]
        public bool setWheelPos = true;
        public bool rotateX;
        public bool rotateY;
        public bool rotateZ;
        public Transform[] wheels;

        [Header("Controller Settings")]
        public TegridyBanditController controller;

        [Header("Time Settings")]
        [Range(1, 3)] public float minSpinTime;
        [Range(3, 10)] public float maxSpinTime;
        [Range(0.01f, 1)] public float spinChange;
        [Range(0.005f, 1)] public float spinUpdate;
        [Range(0, 5)] public float spinDelay;

        [Header("Audio Settings")]
        public AudioClip[] music;
        public AudioClip[] spinClick;
        public AudioClip[] spinsound;
        public AudioClip[] stopSound;
        public AudioClip[] defaultWin;

        Quaternion[] startRotations;
        float[] currentRotations;

        bool spinning;
        public void StartUp()
        {
            //check our config matches
            if (wheels.Length == controller.bandit.wheels.Length) controller.BanditConfig(controller.bandit);
            else { Debug.Log("Wheel mismatch"); return; }

            if (maxSpinTime < minSpinTime) maxSpinTime = minSpinTime;

            for(int i = 0; i < controller.bandit.prizes.Length; i++)
            {
                if(controller.bandit.prizes[i].prizeSound == null)
                    controller.bandit.prizes[i].prizeSound = defaultWin;
            }

            startRotations = new Quaternion[wheels.Length];
            currentRotations = new float[wheels.Length];
            for (int i = 0; i < wheels.Length; i++)
            {
                startRotations[i] = wheels[i].transform.rotation;
                if (setWheelPos)
                {
                    //set the symbols position
                    float div = 360 / controller.bandit.wheels[i].symbol.Length;
                    float thisDiv = controller.bandit.wheels[i].offset;
                    for (int i2 = 0; i2 < controller.bandit.wheels[i].symbol.Length; i2++)
                    {
                        thisDiv %= 360;
                        controller.bandit.wheels[i].symbol[i2].position = thisDiv;
                        thisDiv += div;
                    }
                }
            }
        }
        public BanditResults SpinWheel(float stake)
        {
            if (!spinning)
            {
                StopAllCoroutines();
                BanditResults results = controller.SpinWheels(stake);
                spinning = true;
                StartCoroutine(Spin(results));
                return results;
            }
            else return null;
        }
        IEnumerator Spin(BanditResults results)
        {
            float time = Time.time + Random.Range(minSpinTime, maxSpinTime);

            //spin the wheels a little
            while(time > Time.time)
            {
                for(int i = 0; i < wheels.Length; i++)
                {
                    Quaternion newRotation = wheels[i].rotation;
                    Vector3 thisRotation = new Vector3(newRotation.eulerAngles.x, newRotation.eulerAngles.y, newRotation.eulerAngles.z);
                    currentRotations[i] = (currentRotations[i] + spinChange) % 360;
                    
                    if (rotateX) thisRotation.x = currentRotations[i];
                    if (rotateY) thisRotation.y = currentRotations[i];
                    if (rotateZ) thisRotation.z = currentRotations[i];
                    wheels[i].rotation = Quaternion.Euler(thisRotation);
                }
                yield return new WaitForSeconds(spinUpdate);
            }
            //move to our results
            bool complete = false;
            while (!complete)
            {
                complete = true;
                for (int i = 0; i < wheels.Length; i++)
                {
                    Quaternion newRotation = wheels[i].rotation;
                    Vector3 thisRotation = new Vector3(newRotation.eulerAngles.x, newRotation.eulerAngles.y, newRotation.eulerAngles.z);

                    if (currentRotations[i] == 360) currentRotations[i] = 0;
                    if (currentRotations[i] != controller.bandit.wheels[i].symbol[results.wheelPositions[i]].position)
                    {
                        currentRotations[i] = (currentRotations[i] + spinChange);
                        complete = false;
                        if (rotateX) thisRotation.x = currentRotations[i];
                        if (rotateY) thisRotation.y = currentRotations[i];
                        if (rotateZ) thisRotation.z = currentRotations[i];
                    }
                    wheels[i].rotation = Quaternion.Euler(thisRotation);
                }
                yield return new WaitForSeconds(spinUpdate);
                
            }
            yield return new WaitForSeconds(spinDelay);
            if (complete) spinning = false;
        }
    }
}
