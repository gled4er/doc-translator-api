﻿// // ----------------------------------------------------------------------
// // <copyright file="AccountViewModel.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>AccountViewModel.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.DocumentTranslationInterface.ViewModel
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    using Microsoft.Practices.Prism.Commands;

    using TranslationAssistant.Business;
    using TranslationAssistant.Business.Model;
    using TranslationAssistant.DocumentTranslationInterface.Common;

    /// <summary>
    ///     The account view model.
    /// </summary>
    public class AdvancedViewModel : Notifyer
    {
        #region Fields

        /// <summary>
        ///     The Server Address.
        /// </summary>
        private string EndPointAddress;

        /// <summary>
        ///     The AppId.
        /// </summary>
        private string appId;

        /// <summary>
        ///     The category identifier.
        /// </summary>
        private string adv_categoryID;


        private bool UseAdvancedSettings;

        /// <summary>
        ///     The save account settings click command.
        /// </summary>
        private ICommand saveAccountSettingsClickCommand;

        /// <summary>
        ///     The status text.
        /// </summary>
        private string statusText;

        #endregion

        #region Constructors and Destructors


        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the appid.
        /// </summary>
        public string AppId
        {
            get
            {
                return this.appId;
            }

            set
            {
                this.appId= value;
                this.NotifyPropertyChanged("AppID");
            }
        }
        public string Adv_CategoryID
        {
            get
            {
                return this.adv_categoryID;
            }

            set
            {
                this.adv_categoryID = value;
                this.NotifyPropertyChanged("Adv_CategoryID");
            }
        }

        /// <summary>
        ///     Gets the save account settings click command.
        /// </summary>
        public ICommand SaveAccountSettingsClickCommand
        {
            get
            {
                return this.saveAccountSettingsClickCommand
                       ?? (this.saveAccountSettingsClickCommand = new DelegateCommand(this.SaveAccountClick));
            }
        }

        /// <summary>
        ///     Gets or sets the status text.
        /// </summary>
        public string StatusText
        {
            get
            {
                return this.statusText;
            }

            set
            {
                this.statusText = value;
                this.NotifyPropertyChanged("StatusText");
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Load the account settings from the DocumentTranslator.settings file, which is actually in the user appsettings folder and named user.config.
        /// </summary>
        public AdvancedViewModel()
        {
            //Initialize in order to load the credentials.
            TranslationServices.Core.TranslationServiceFacade.Initialize();
            this.appId = TranslationServices.Core.TranslationServiceFacade.AppId;
            this.adv_categoryID = TranslationServices.Core.TranslationServiceFacade.Adv_CategoryId;
            this.EndPointAddress= TranslationServices.Core.TranslationServiceFacade.EndPointAddress;
            this.UseAdvancedSettings = TranslationServices.Core.TranslationServiceFacade.UseAdvancedSettings;
        }

        /// <summary>
        ///     Saves the account settings to the settings file for next use.
        /// </summary>
        private void SaveAccountClick()
        {
            //Set the Account values and save.
            TranslationServices.Core.TranslationServiceFacade.ClientID = TranslationServices.Core.TranslationServiceFacade.ClientID.Trim();
            TranslationServices.Core.TranslationServiceFacade.CategoryID = this.adv_categoryID.Trim();
            TranslationServices.Core.TranslationServiceFacade.SaveCredentials();
            TranslationServices.Core.TranslationServiceFacade.Initialize();
            
            if (TranslationServices.Core.TranslationServiceFacade.IsTranslationServiceReady()) { 
                this.StatusText = "Settings saved. Ready to translate.";
                NotifyPropertyChanged("SettingsSaved");
                //Need to initialize with new credentials in order to get the language list.
                SingletonEventAggregator.Instance.GetEvent<AccountValidationEvent>().Publish(true);
            }
            else
            {
                this.StatusText = "Key is invalid.\r\nPlease visit the Azure Portal to obtain a subscription key.";
                SingletonEventAggregator.Instance.GetEvent<AccountValidationEvent>().Publish(false);
                return;
            }
            if (!TranslationServices.Core.TranslationServiceFacade.IsCategoryValid(this.adv_categoryID))
            {
                this.StatusText = "Category is invalid.\r\nPlease visit https://hub.microsofttranslator.com to determine a valid category ID, leave empty, or use one of the standard categories.";
                SingletonEventAggregator.Instance.GetEvent<AccountValidationEvent>().Publish(false);
            }
            return;
          
        }

        #endregion
    }
}