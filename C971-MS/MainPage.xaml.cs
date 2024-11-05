namespace C971_MS
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        DatabaseLogic db = new();
        List<Course> courseList = new();
        int currentTerm;

        public MainPage(int termID = 1)
        {
            InitializeComponent();

            currentTerm = termID;

            GetCourseList();
        }
        async void GetCourseList()
        {
            Term temp = await db.LookupTermByID(currentTerm);
            TermLabel.Text = $"{temp.TermName} : {temp.StartDate.Date.ToString("d")} - {temp.EndDate.Date.ToString("d")}";
            courseList = await db.GetTermInfo(currentTerm);

            foreach (Course item in courseList)
            {
                var btn = new Button { Text = $"{item.CourseName}" };
                btn.StyleId = item.ID.ToString();
                btn.Clicked += OnButtonClicked;
                btn.Background = Colors.LightGray;
                btn.BorderWidth = 1;
                btn.BorderColor = Colors.Black;
                btn.TextColor = Colors.Black;
                CourseContainer.Children.Add(btn);
                count++;
            }
        }
       
        private async void OnButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            Course temp = await db.LookupCourseByID(Convert.ToInt32(button.StyleId));
            
            await Navigation.PushModalAsync(new CourseDetails(temp));
        }

        private void TermBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new AllTerms());
        }

        private void EditBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new EditTerm(currentTerm));
        }

        private async void AddBtn_Clicked(object sender, EventArgs e)
        {
            if (count < 6)
            {
                await Navigation.PushModalAsync(new EditCourse(false, currentTerm, new Course()));
            }
            else
            {
                await DisplayAlert("Error", "Cannot have more than 6 classes!", "OK");
            }
        }

    }
}