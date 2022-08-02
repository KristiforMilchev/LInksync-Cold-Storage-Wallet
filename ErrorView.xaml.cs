using Microsoft.AspNetCore.Components;
using System.Timers;

namespace SYNCWallet;

public partial class ErrorView : ContentPage
{

	public string Message { get; set; }
	public ErrorView()
	{
		InitializeComponent();
    }

    public ErrorView(string text, int attempts)
    {
        Message = text;
        InitializeComponent();

        ErrorLabel.Text = text;
    }


 

    
}