using Microsoft.IdentityModel.Tokens;

namespace C971_MS;

public partial class EditTermDates : ContentPage
{
    Term CurrentTerm;
    DatabaseLogic db = new();

    bool ismod;
    public EditTermDates(bool IsMod, Term term)
	{
		InitializeComponent();
       
        CurrentTerm = term;
        this.BindingContext = CurrentTerm;


        ismod = IsMod;
    }

    private async void SaveBtn_Clicked(object sender, EventArgs e)
    {
        if (StartDatePicker.Date >= EndDatePicker.Date)
        {
            await DisplayAlert("Error", "End date must be after start date!", "OK");
        }
        else
        {
            if (ismod)
            {
                db.UpdateTerm(CurrentTerm);
            }
            else
            {
                if (CurrentTerm.TermName.IsNullOrEmpty())
                    {
                        List<Term> termList = await db.GetTerms();
                        CurrentTerm.TermName = $"Term {termList.Count + 1}";
                    }
                db.AddTerm(CurrentTerm);
            }
            await Navigation.PushModalAsync(new AllTerms());
        }

    }

    private async void CancelBtn_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new AllTerms());
    }
}