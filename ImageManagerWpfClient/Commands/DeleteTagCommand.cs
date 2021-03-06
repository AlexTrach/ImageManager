﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ImagesWcfServiceClient.Models;
using System.Windows.Input;

namespace ImageManagerWpfClient
{
    class DeleteTagCommand : ICommand
    {
        public AvailableTagsEditingWindowViewModel ViewModel { get; set; }

        public event EventHandler CanExecuteChanged;

        public DeleteTagCommand(AvailableTagsEditingWindowViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Tag tagToDelete = (Tag) parameter;

            if (tagToDelete.Id == ViewModel.TagToUpdate?.Id)
            {
                ViewModel.CanEnterTagNameToUpdate = false;
                ViewModel.TagNameToUpdate = null;
            }
            
            ServiceClientWrapper.Instance.DeleteTag(tagToDelete);
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged(this, e);
        }
    }
}
