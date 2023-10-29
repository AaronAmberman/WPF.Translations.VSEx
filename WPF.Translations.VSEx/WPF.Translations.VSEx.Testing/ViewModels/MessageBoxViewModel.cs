﻿using System;
using System.Windows;
using WPF.InternalDialogs;

namespace WPF.Translations.VSEx.Testing.ViewModels
{
    internal class MessageBoxViewModel : ViewModelBase
    {
        #region Fields

        private MessageBoxButton messageBoxButton = MessageBoxButton.OK;
        private MessageBoxInternalDialogImage messageBoxImage = MessageBoxInternalDialogImage.Information;
        private bool messageBoxIsModal;
        private string messageBoxMessage;
        private MessageBoxResult messageBoxResult;
        private string messageBoxTitle;
        private Visibility messageBoxVisibility = Visibility.Collapsed;

        #endregion

        #region Properties

        public Action CloseAction { get; set; }

        public MessageBoxButton MessageBoxButton
        {
            get => messageBoxButton;
            set
            {
                messageBoxButton = value;
                OnPropertyChanged();
            }
        }

        public MessageBoxInternalDialogImage MessageBoxImage
        {
            get => messageBoxImage;
            set
            {
                messageBoxImage = value;
                OnPropertyChanged();
            }
        }

        public bool MessageBoxIsModal
        {
            get => messageBoxIsModal;
            set
            {
                messageBoxIsModal = value;
                OnPropertyChanged();
            }
        }

        public string MessageBoxMessage
        {
            get => messageBoxMessage;
            set
            {
                messageBoxMessage = value;
                OnPropertyChanged();
            }
        }

        public MessageBoxResult MessageBoxResult
        {
            get => messageBoxResult;
            set
            {
                messageBoxResult = value;
                OnPropertyChanged();
            }
        }

        public string MessageBoxTitle
        {
            get => messageBoxTitle;
            set
            {
                messageBoxTitle = value;
                OnPropertyChanged();
            }
        }

        public Visibility MessageBoxVisibility
        {
            get => messageBoxVisibility;
            set
            {
                messageBoxVisibility = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
