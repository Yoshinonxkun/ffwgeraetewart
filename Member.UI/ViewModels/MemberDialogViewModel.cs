using System;
using Member.Data;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Member.UI.ViewModels
{
    public class MemberDialogViewModel : BindableBase, IDialogAware
    {
        private DelegateCommand<string> _closeDialogCommand;

        private Data.Member _member;

        private Psa _psa;

        private string _title = "Bearbeiten";

        public MemberDialogViewModel()
        {
            AddMonthCommand = new DelegateCommand(AddMonth);
            SubMonthCommand = new DelegateCommand(SubMonth);
            AddYearCommand = new DelegateCommand(AddYear);
            SubYearCommand = new DelegateCommand(SubYear);
        }

        public DelegateCommand<string> CloseDialogCommand =>
            _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand<string>(CloseDialog));

        public Data.Member Member
        {
            get => _member;
            set => SetProperty(ref _member, value);
        }

        public Psa Psa
        {
            get => _psa;
            set => SetProperty(ref _psa, value);
        }

        public DelegateCommand AddMonthCommand { get; set; }
        public DelegateCommand SubMonthCommand { get; set; }
        public DelegateCommand AddYearCommand { get; set; }
        public DelegateCommand SubYearCommand { get; set; }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public event Action<IDialogResult> RequestClose;

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {
        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            Member = parameters.GetValue<Data.Member>("member");
            Psa = parameters.GetValue<Psa>("psa");
        }

        protected virtual void CloseDialog(string parameter)
        {
            var result = ButtonResult.None;

            if (parameter?.ToLower() == "true")
                result = ButtonResult.OK;
            else if (parameter?.ToLower() == "false")
                result = ButtonResult.Cancel;

            RaiseRequestClose(new DialogResult(result));
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        private void AddMonth()
        {
            Psa.HelmDate = Psa.HelmDate.AddMonths(1);
            RaisePropertyChanged("Psa");
        }

        private void SubMonth()
        {
            Psa.HelmDate = Psa.HelmDate.AddMonths(-1);
            RaisePropertyChanged("Psa");
        }

        private void AddYear()
        {
            Psa.HelmDate = Psa.HelmDate.AddYears(1);
            RaisePropertyChanged("Psa");
        }

        private void SubYear()
        {
            Psa.HelmDate = Psa.HelmDate.AddYears(-1);
            RaisePropertyChanged("Psa");
        }
    }
}