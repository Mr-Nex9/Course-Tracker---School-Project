namespace C971_MS;

public partial class AllTerms : ContentPage
{
    DatabaseLogic db = new();
    List<Term> termList = new();

    public AllTerms()
    {
        InitializeComponent();



    }
    protected override void OnAppearing()
    {
        GetTermList();
    }
    async void GetTermList()
    {
        await db.LoadDatabase();

        termList = await db.GetTerms();
       
        foreach (Term term in termList)
        {
            Console.WriteLine(term.TermName);

            var btn = new Button { Text = $"{term.TermName}: {term.StartDate.Date.ToString("d")} -{term.EndDate.Date.ToString("d")} "};
            btn.Clicked += OnButtonClicked;
            btn.Background = Colors.LightGray;
            btn.BorderWidth = 1;
            btn.BorderColor = Colors.Black;
            btn.TextColor = Colors.Black;
            TermContainer.Children.Add(btn);
        }
    }
    private void TermBtn_Clicked(object sender, EventArgs e)
    {
        Term temp = new();
        Navigation.PushModalAsync(new EditTermDates(false, temp));
    }

    private void EditBtn_Clicked(object sender, EventArgs e)
    {
        Navigation.PushModalAsync(new EditAllTerms());
    }

    private async void OnButtonClicked(object? sender, EventArgs e)
    {
        var button = sender as Button;
        int pos = button.Text.IndexOf(':');
        int termID = await db.GetTermID(button.Text[..pos]);

        await Navigation.PushModalAsync(new MainPage(termID));
    }
}