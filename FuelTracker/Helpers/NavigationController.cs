﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace FuelTracker.Services
{
    public class NavigationController
    {
        public NavigationController()
        {

        }

        private PhoneApplicationFrame GetRootPhoneApplicationFrame()
        {
            PhoneApplicationFrame applicationFrame = (Application.Current.RootVisual as PhoneApplicationFrame);
            return applicationFrame;
        }

        public void Navigate(Uri address)
        {
            PhoneApplicationFrame applicationFrame = GetRootPhoneApplicationFrame();
            if (applicationFrame == null)
            {
                throw new NullReferenceException("applicationFrame must not be null!");
            }

            applicationFrame.Navigate(address);
        }

        public void GoBack()
        {
            PhoneApplicationFrame applicationFrame = GetRootPhoneApplicationFrame();
            if (applicationFrame == null)
            {
                throw new NullReferenceException("applicationFrame must not be null!");
            }

            if (applicationFrame.CanGoBack)
            {
                applicationFrame.GoBack();
            }
        }

        public void GoForward()
        {
            PhoneApplicationFrame applicationFrame = GetRootPhoneApplicationFrame();
            if (applicationFrame == null)
            {
                throw new NullReferenceException("applicationFrame must not be null!");
            }

            applicationFrame.GoForward();
        }

        public string GetCurrentUri()
        {
            PhoneApplicationFrame applicationFrame = GetRootPhoneApplicationFrame();
            if (applicationFrame == null)
            {
                throw new NullReferenceException("applicationFrame must not be null!");
            }

            string currentUri = applicationFrame.CurrentSource.OriginalString;
            return currentUri;
        }

        private static NavigationController _instanceNavigationController;
        public static NavigationController Instance
        {
            get
            {
                if (_instanceNavigationController == null)
                {
					_instanceNavigationController = new NavigationController();
                }

                return _instanceNavigationController;
            }
        }

    }
}
