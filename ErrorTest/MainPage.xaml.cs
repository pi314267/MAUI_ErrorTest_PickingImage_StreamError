using ErrorTest.ViewModel;

namespace ErrorTest;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();

        var vm = new MainViewModel();
        this.BindingContext = vm;

    }

}

