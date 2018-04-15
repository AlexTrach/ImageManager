﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ImagesWcfServiceClient.Models;

namespace ImageManagerWpfClient
{
    class SelectTagToUpdateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public AvailableTagsEditingWindowViewModel ViewModel { get; set; }

        public SelectTagToUpdateCommand(AvailableTagsEditingWindowViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Tag tagToUpdate = (Tag) parameter;

            ViewModel.TagToUpdate = tagToUpdate;
            ViewModel.CanEnterTagNameToUpdate = true;
        }

        protected void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged(this, e);
        }
    }
}
