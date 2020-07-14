using System.Collections.Generic;
using Member.Data.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using PsaDruck.WordLogic;

namespace PsaDruck.UI.ViewModels
{
    public class ComboboxOptions
    {
        public int Id { get; set; }
        public string Option { get; set; }
    }

    public class PsaDruckViewModel : BindableBase, INavigationAware
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IPsaRepository _psaRepository;
        private readonly IRegionManager _regionManager;
        private Member.Data.Member _selectedMember;
        private ComboboxOptions _selectedOption;

        public PsaDruckViewModel(IRegionManager regionManager,
            IMemberRepository memberRepository, IPsaRepository psaRepository)
        {
            _memberRepository = memberRepository;
            _regionManager = regionManager;
            _psaRepository = psaRepository;

            AvailableMembers = _memberRepository.GetMembers();
            SelectedMember = null;

            Options = new List<ComboboxOptions>
            {
                new ComboboxOptions {Id = 1, Option = "arbeitskleidung"},
                new ComboboxOptions {Id = 2, Option = "einsatzkleidung"},
                new ComboboxOptions {Id = 3, Option = "handschuhe"},
                new ComboboxOptions {Id = 4, Option = "helm"},
                new ComboboxOptions {Id = 5, Option = "kopfschutzhaube"},
                new ComboboxOptions {Id = 6, Option = "schuhe"}
            };

            NavigateCommand = new DelegateCommand<string>(Navigate);
            PrintSpecialCommand = new DelegateCommand(PrintSpecial);
            PrintByMemberCommand = new DelegateCommand(PrintByMember);
            PrintAllCommand = new DelegateCommand(PrintAll);
            PrintExcelCommand = new DelegateCommand(PrintExcel);
        }

        public DelegateCommand<string> NavigateCommand { get; }
        public DelegateCommand PrintSpecialCommand { get; }
        public DelegateCommand PrintByMemberCommand { get; }
        public DelegateCommand PrintAllCommand { get; }
        public DelegateCommand PrintExcelCommand { get; }

        public IEnumerable<Member.Data.Member> AvailableMembers { get; set; }

        public Member.Data.Member SelectedMember
        {
            get => _selectedMember;
            set
            {
                _selectedMember = value;
                RaisePropertyChanged();
            }
        }

        public IEnumerable<ComboboxOptions> Options { get; set; }

        public ComboboxOptions SelectedOption
        {
            get => _selectedOption;
            set
            {
                _selectedOption = value;
                RaisePropertyChanged();
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            // es wird hinnavigiert
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            AvailableMembers = _memberRepository.GetMembers();
            RaisePropertyChanged("AvailableMembers");
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            // es wird wegnavigiert
        }

        private void Navigate(string navigatePath)
        {
            if (navigatePath != null)
                _regionManager.RequestNavigate("MainRegion", navigatePath);
        }

        private void PrintAll()
        {
            var wordLogic = new MyWordLogic();
            wordLogic.GetAll(AvailableMembers, _psaRepository);
            wordLogic.GetFile();
        }

        private void PrintByMember()
        {
            if (SelectedMember == null) return;
            var wordLogic = new MyWordLogic();
            wordLogic.GetAllByMember(SelectedMember, _psaRepository.GetPsaByMember(SelectedMember));
            wordLogic.GetFile();
        }

        private void PrintSpecial()
        {
            if (SelectedMember == null) return;
            var wordLogic = new MyWordLogic();
            wordLogic.GetSingle(SelectedOption.Option, SelectedMember, _psaRepository.GetPsaByMember(SelectedMember));
            wordLogic.GetFile();
        }

        private void PrintExcel()
        {
            var wordLogic = new MyWordLogic();
            wordLogic.GetExcelList(AvailableMembers, _psaRepository);
        }
    }
}