﻿using System;
using System.Collections.Generic;
using System.Text;

namespace prj3beer.Services
{
    public class Notifications
    {
        // These arrays have values in place to correspond to match the Notification type gotten from TryNotification
        // This is so that the notification send can more easily pull from a pre-made list of titles and bodys
        public static string[] Title = { "Message Error", "Heat Warning", "Temperature Alert", "Drink Time!", "Temperature Alert", "Cold Warning" };
        public static string[] Body = { "This should not be seen", "Your beverage is getting too hot.", "Your beverage is just above the desired temperature.", "Your beverage has reached the perfect temperature.", "Your beverage is just below the desired temperature.", "Your beverage is getting too cold." };

        //This variable will contain the last notification that has been sent in order to compare
        //to control notification functionality
        public static NotificationType lastNotification = NotificationType.NO_MESSAGE;

        /// <summary>
        /// Calling this method will give a number based on the NotificationType Enumeration
        ///     to be used to select the corresponding title and body of a notification form the arrays above
        /// </summary>
        /// <param name="receivedTemp">The temperature last recived from the bluetooth device</param>
        /// <param name="idealTemp">The set ideal temperature of the user's currently selected beverage</param>
        /// <param name="lastNotification">The last notification that was sent so this method can compare them and not spam the user</param>
        /// <returns>The notification type based on the temperature. Returns 0 if no notification should be sent</returns>
        static public int TryNotification(double receivedTemp, double? idealTemp, NotificationType lastNotification)
        {
            NotificationType newNotification = CompareTemp(receivedTemp, idealTemp);//Sets the current notification type that would be sent

            if (lastNotification == NotificationType.NO_MESSAGE)//If there wasn't a last type of message, set it to prevent initial TOO_HOT or TOO_COLD messages
            {
                if (receivedTemp > idealTemp)
                {
                    lastNotification = NotificationType.TOO_HOT;
                }
                else
                {
                    lastNotification = NotificationType.TOO_COLD;
                }
            }

            if (CheckLastNotification(newNotification, lastNotification))//If this returns true because it's appropriate to send a new notification (this is the spam check)
            {
                return (int)newNotification;//An int to use for getting messages from the Title and body arrays
            }
            return 0;//Should not send a new notification
        }


        /// <summary>
        /// A helper method to determine the notification type that should be selected based on the recived temperature
        /// </summary>
        /// <param name="receivedTemp">The temperature last gotten from the bluetooth device</param>
        /// <param name="idealTemp">The set ideal temperature of the user's currently selected beverage</param>
        /// <returns>The notification type that is relevent to the received temperature based on their difference</returns>
        static private NotificationType CompareTemp(double receivedTemp, double? idealTemp)
        {
            int dif = (int)( receivedTemp - idealTemp );

            NotificationType curType = default;


            if (dif == 0) //The temperatures are equal
                curType = NotificationType.PERFECT;

            else if (dif == 1 || dif == 2 ) // The received temperature is within two degrees above the ideal temperature
                curType = NotificationType.IN_RANGE_HOT;

            else if (dif == -1 || dif == -2) // The received temperature is within two degrees below the ideal temperature
                curType = NotificationType.IN_RANGE_COLD;

            else if (dif >= 4) // The received temperature is more than four degrees above the ideal temperature
                curType = NotificationType.TOO_HOT;

            else if (dif <= -4) // The received temperature is more than four degrees below the ideal temperature
                curType = NotificationType.TOO_COLD;

            return curType;
        }

        /// <summary>
        /// Compares the last notification to what the current notification gotten from CompareTemp would be
        /// </summary>
        /// <param name="current">The current notification type gotten from CompareTemp</param>
        /// <param name="last">The last notification type that was sent</param>
        /// <returns>True if a notification should be sent, false if a notification should not be sent</returns>
        static private bool CheckLastNotification(NotificationType current, NotificationType last)
        {
            bool sendable = true;

            if(current == last)//Will not send two of the same notification
            {
                sendable = false;
            }
            else if ((current != NotificationType.TOO_COLD && current != NotificationType.TOO_HOT) && last == NotificationType.PERFECT)
            {// Will not send an in_range notification if the last was a perfect notification
                sendable = false;
            }
            
            return sendable;
        }
    }
}
