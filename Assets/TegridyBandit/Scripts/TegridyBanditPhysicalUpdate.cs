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
    public class TegridyBanditPhysicalUpdate : MonoBehaviour
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
        [Range(0.01f, 5)] public float spinChange;

        [Header("Audio Settings")]
        public AudioClip[] music;
        public AudioClip[] spinClick;
        public AudioClip[] spinsound;
        public AudioClip[] stopSound;
        public AudioClip[] defaultWin;

        Quaternion[] startRotations;
        float[] currentRotations;

        bool spining;
        float timeSpin;
        BanditResults results;
        public void StartUp()
        {
            //check our config matches
            if (wheels.Length == controller.bandit.wheels.Length) controller.BanditConfig(controller.bandit);
            else { Debug.Log("Wheel mismatch"); return; }

            if (maxSpinTime < minSpinTime) maxSpinTime = minSpinTime;

            for (int i = 0; i < controller.bandit.prizes.Length; i++)
            {
                if (controller.bandit.prizes[i].prizeSound == null)
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
            if (!spining)
            {
                StopAllCoroutines();
                results = controller.SpinWheels(stake);

                timeSpin = Time.time + Random.Range(minSpinTime, maxSpinTime);

                spining = true;
                return results;
            }
            else return null;
        }



        private void Update()
        {
            if (spining && timeSpin > Time.time)
            {
                for (int i = 0; i < wheels.Length; i++)
                {
                    Quaternion newRotation = wheels[i].rotation;
                    Vector3 thisRotation = new Vector3(newRotation.eulerAngles.x, newRotation.eulerAngles.y, newRotation.eulerAngles.z);
                    currentRotations[i] = (currentRotations[i] + spinChange) % 360;

                    if (rotateX) thisRotation.x = currentRotations[i];
                    if (rotateY) thisRotation.y = currentRotations[i];
                    if (rotateZ) thisRotation.z = currentRotations[i];
                    wheels[i].rotation = Quaternion.Euler(thisRotation);
                }
            }
            else if (spining && timeSpin < Time.time)
            {
                spining = false;
                for (int i = 0; i < wheels.Length; i++)
                {
                    Quaternion newRotation = wheels[i].rotation;
                    Vector3 thisRotation = new Vector3(newRotation.eulerAngles.x, newRotation.eulerAngles.y, newRotation.eulerAngles.z);

                    if (currentRotations[i] == 360) currentRotations[i] = 0;



                    if (currentRotations[i] + spinChange <= controller.bandit.wheels[i].symbol[results.wheelPositions[i]].position && currentRotations[i] - spinChange >= controller.bandit.wheels[i].symbol[results.wheelPositions[i]].position)

                    //if (currentRotations[i] != controller.bandit.wheels[i].symbol[results.wheelPositions[i]].position)
                    {
                        spining = true;
                        currentRotations[i] = (currentRotations[i] + spinChange);
                        if (rotateX) thisRotation.x = currentRotations[i];
                        if (rotateY) thisRotation.y = currentRotations[i];
                        if (rotateZ) thisRotation.z = currentRotations[i];

                        wheels[i].rotation = Quaternion.Euler(thisRotation);
                    }






                }


            }


        }
    }
}
