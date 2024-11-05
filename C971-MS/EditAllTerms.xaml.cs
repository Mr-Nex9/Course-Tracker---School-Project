namespace C971_MS;

public partial class EditAllTerms : ContentPage
{
    DatabaseLogic db = new();
    List<Term> termList = new();
    Term SelectedTerm;
    Button button;
    public EditAllTerms()
    {
        InitializeComponent();
        GetTermList();
    }
    async void GetTermList()
    {
        await db.LoadDatabase();

        termList = await db.GetTerms();

        TermContainer.Children.Clear();
        foreach (Term term in termList)
        {
            var btn = new Button { Text = $"{term.TermName}: {term.StartDate.Date} -{term.EndDate.Date} " };
            btn.StyleId = term.TermName;
            btn.Clicked += OnButtonClicked;
            btn.Background = Colors.LightGray;
            btn.BorderWidth = 1;
            btn.BorderColor = Colors.Black;
            btn.TextColor = Colors.Black;
            TermContainer.Children.Add(btn);
        }
    }
    private async void OnButtonClicked(object sender, EventArgs e)
    {
        if (button != null)
            button.Background = Colors.LightGray;

        button = (Button)sender;
        button.Background = Colors.Blue;
        SelectedTerm = await db.LookupTermByName(button.StyleId);
    }
    private void SaveBtn_Clicked(object sender, EventArgs e)
    {
        Navigation.PushModalAsync(new AllTerms());
    }

    private async void DeleteBtn_Clicked(object sender, EventArgs e)
    {
        if (SelectedTerm != null)
        {
            db.DeleteTerm(SelectedTerm);
            await Navigation.PushModalAsync(new EditAllTerms()); //refresh page
        }
        else
        {
            await DisplayAlert("Error", "No Term currently selected!", "OK");
        }
    }

    private void EditBtn_Clicked(object sender, EventArgs e)
    {
        Navigation.PushModalAsync(new EditTermDates(true, SelectedTerm));
    }
}