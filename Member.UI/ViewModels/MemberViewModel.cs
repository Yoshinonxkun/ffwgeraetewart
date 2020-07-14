using System.Collections.Generic;
using Member.Data;
using Member.Data.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace Member.UI.ViewModels
{
    public class MemberViewModel : BindableBase
    {
        private readonly IDialogService _dialogService;
        private readonly IMemberRepository _memberRepository;
        private readonly IPsaRepository _psaRepository;
        private readonly IRegionManager _regionManager;
        private Data.Member _selectedMember;
        private Psa _selectedPsa;

        public MemberViewModel(IRegionManager regionManager, IDialogService dialogService,
            IMemberRepository memberRepository, IPsaRepository psaRepository)
        {
            _regionManager = regionManager;
            _dialogService = dialogService;
            _memberRepository = memberRepository;
            _psaRepository = psaRepository;

            NavigateCommand = new DelegateCommand<string>(Navigate);
            CreateNewMemberCommand = new DelegateCommand(CreateNewMember);
            EditMemberCommand = new DelegateCommand(EditMember);
            DeleteSelectedMemberCommand = new DelegateCommand(DeleteSelectedMember);

            AvailableMembers = _memberRepository.GetMembers();
        }

        public DelegateCommand<string> NavigateCommand { get; }
        public DelegateCommand CreateNewMemberCommand { get; }
        public DelegateCommand EditMemberCommand { get; }
        public DelegateCommand DeleteSelectedMemberCommand { get; }
        public IEnumerable<Data.Member> AvailableMembers { get; set; }

        public Data.Member SelectedMember
        {
            get => _selectedMember;
            set
            {
                _selectedMember = value;
                SelectedPsa = _psaRepository.GetPsaByMember(_selectedMember);
                RaisePropertyChanged();
            }
        }

        public Psa SelectedPsa
        {
            get => _selectedPsa;
            set
            {
                _selectedPsa = value;
                RaisePropertyChanged();
            }
        }

        public string SurnameEntry { get; set; }
        public string NameEntry { get; set; }

        private void Navigate(string navigatePath)
        {
            if (navigatePath != null)
                _regionManager.RequestNavigate("MainRegion", navigatePath);
        }

        private void CreateNewMember()
        {
            var newMember = new Data.Member {Surname = SurnameEntry, Name = NameEntry};
            _memberRepository.InsertMember(newMember);

            AvailableMembers = _memberRepository.GetMembers();
            RaisePropertyChanged("AvailableMembers");
        }

        private void EditMember()
        {
            var parameters = new DialogParameters
            {
                {"member", SelectedMember},
                {"psa", SelectedPsa}
            };
            _dialogService.ShowDialog("MemberDialogView", parameters, result =>
            {
                if (result.Result == ButtonResult.None)
                {
                    //System.Console.Out.WriteLine("Result is None");
                }
                else if (result.Result == ButtonResult.OK)
                {
                    //System.Console.Out.WriteLine("Result is OK");
                    _memberRepository.UpdateMember(SelectedMember);
                    _psaRepository.UpdatePsa(SelectedPsa);
                }
                else if (result.Result == ButtonResult.Cancel)
                {
                    //System.Console.Out.WriteLine("Result is Cancel");
                }
            });

            AvailableMembers = _memberRepository.GetMembers();

            RaisePropertyChanged("AvailableMembers");
            RaisePropertyChanged("SelectedMember");
            RaisePropertyChanged("SelectedPsa");
        }

        private void DeleteSelectedMember()
        {
            var parameters = new DialogParameters
            {
                {"member", SelectedMember}
            };
            _dialogService.ShowDialog("MemberDeleteDialogView", parameters, result =>
            {
                if (result.Result == ButtonResult.None)
                {
                    //System.Console.Out.WriteLine("Result is None");
                }
                else if (result.Result == ButtonResult.OK)
                {
                    //System.Console.Out.WriteLine("Result is OK");
                    _memberRepository.DeleteMember(SelectedMember);
                    SelectedMember = new Data.Member();
                    AvailableMembers = _memberRepository.GetMembers();
                }
                else if (result.Result == ButtonResult.Cancel)
                {
                    //System.Console.Out.WriteLine("Result is Cancel");
                }
            });

            RaisePropertyChanged("AvailableMembers");
        }
    }
}