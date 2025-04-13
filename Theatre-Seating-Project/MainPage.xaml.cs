using System.Collections.Specialized;
using System.Reflection.Metadata;

namespace Theatre_Seating_Project;

    public class SeatingUnit
    {
        public string Name { get; set; }
        public bool Reserved { get; set; }

        public SeatingUnit(string name, bool reserved = false)
        {
            Name = name;
            Reserved = reserved;
        }
    }

    public partial class MainPage : ContentPage
    {
        SeatingUnit[,] seatingChart = new SeatingUnit[5, 10];

        public MainPage()
        {
            InitializeComponent();
            GenerateSeatingNames();
            RefreshSeating();
        }

        private async void ButtonReserveSeat(object sender, EventArgs e)
        {
            var seat = await DisplayPromptAsync("Enter Seat Number", "Enter seat number: ");

            if (seat != null)
            {
                for (int i = 0; i < seatingChart.GetLength(0); i++)
                {
                    for (int j = 0; j < seatingChart.GetLength(1); j++)
                    {
                        if (seatingChart[i, j].Name == seat)
                        {
                            seatingChart[i, j].Reserved = true;
                            await DisplayAlert("Successfully Reserverd", "Your seat was reserverd successfully!", "Ok");
                            RefreshSeating();
                            return;
                        }
                    }
                }
                await DisplayAlert("Error", "Seat was not found.", "Ok");
            }
        }

        private void GenerateSeatingNames()
        {
            List<string> letters = new List<string>();
            for (char c = 'A'; c <= 'Z'; c++)
            {
                letters.Add(c.ToString());
            }

            int letterIndex = 0;

            for (int row = 0; row < seatingChart.GetLength(0); row++)
            {
                for (int column = 0; column < seatingChart.GetLength(1); column++)
                {
                    seatingChart[row, column] = new SeatingUnit(letters[letterIndex] + (column + 1).ToString());
                }
                letterIndex++;
            }
        }

        private void RefreshSeating()
        {
            grdSeatingView.RowDefinitions.Clear();
            grdSeatingView.ColumnDefinitions.Clear();
            grdSeatingView.Children.Clear();

            for (int row = 0; row < seatingChart.GetLength(0); row++)
            {
                var grdRow = new RowDefinition();
                grdRow.Height = 50;

                grdSeatingView.RowDefinitions.Add(grdRow);

                for (int column = 0; column < seatingChart.GetLength(1); column++)
                {
                    var grdColumn = new ColumnDefinition();
                    grdColumn.Width = 50;

                    grdSeatingView.ColumnDefinitions.Add(grdColumn);

                    var text = seatingChart[row, column].Name;

                    var seatLabel = new Label();
                    seatLabel.Text = text;
                    seatLabel.HorizontalOptions = LayoutOptions.Center;
                    seatLabel.VerticalOptions = LayoutOptions.Center;
                    seatLabel.BackgroundColor = Color.Parse("#333388");
                    seatLabel.Padding = 10;

                    if (seatingChart[row, column].Reserved == true)
                    {
                        //Change the color of this seat to represent its reserved.
                        seatLabel.BackgroundColor = Color.Parse("#883333");

                    }
                    Grid.SetRow(seatLabel, row);
                    Grid.SetColumn(seatLabel, column);
                    grdSeatingView.Children.Add(seatLabel);
                }
            }
        }

        // Implemented by Pappu Jha
        private async void ButtonReserveRange(object sender, EventArgs e)
        {
            var startSeatNum = await DisplayPromptAsync("Reserve Seat Range", "Enter the STARTING seat number:");
            var endSeatNum = await DisplayPromptAsync("Reserve Seat Range", "Enter the ENDING seat number:");

            if (string.IsNullOrEmpty(startSeatNum) || string.IsNullOrEmpty(endSeatNum))
            {
                await DisplayAlert("ERROR!", "EMPTY Seat Numbers.", "OK");
                return;
            }

            bool locateStart = false, locateEnd = false;
            int startRowNum = -1, startColNum = -1, endRowNum = -1, endColNum = -1;

            int m = 0;
            while (m < seatingChart.GetLength(0))
            {
                int n = 0;
                while (n < seatingChart.GetLength(1))
                {
                    if (seatingChart[m, n].Name == startSeatNum)
                    {
                        startRowNum = m;
                        startColNum = n;
                        locateStart = true;
                    }
                    if (seatingChart[m, n].Name == endSeatNum)
                    {
                        endRowNum = m;
                        endColNum = n;
                        locateEnd = true;
                    }
                    n++;
                }
                m++;
            }

            if (locateStart == false || locateEnd == false)
            {
                await DisplayAlert("ERROR!", "Either start or end seat or both NOT found.", "OK");
                return;
            }

            if (startRowNum != endRowNum)
            {
                await DisplayAlert("ERROR!", "SEATS are NOT in the same row.", "OK");
                return;
            }

            for (int p = startColNum; p <= endColNum; p++)
            {
                if (seatingChart[startRowNum, p].Reserved)
                {
                    await DisplayAlert("ERROR!", $"Seat {seatingChart[startRowNum, p].Name} has already been reserved.", "OK");
                    return;
                }
            }
            for (int w = startColNum; w <= endColNum; w++)
            {
                seatingChart[startRowNum, w].Reserved = true;
            }

            RefreshSeating();
            await DisplayAlert("SUCCESS!", "Seats have been reserved successfully!", "OK");
        }

        // Implemented by Dol Raj Bashyal
        private async void ButtonCancelReservation(object sender, EventArgs e)
        {
            //getting seat number from the user
            var seat = await DisplayPromptAsync("Enter Seat Number", "Enter seat number: ");

            if (seat != null)
            {   
                bool correctSeat = false;

                //searching seat in chart
                for (int i = 0; i < seatingChart.GetLength(0); i++)
                {
                    for (int j = 0; j < seatingChart.GetLength(1); j++)
                    {
                        if (seatingChart[i, j].Name == seat)
                        {  
                            correctSeat = true;
                            if (seatingChart[i,j].Reserved)
                            { //if seat found, cancel reservation
                                seatingChart[i, j].Reserved = false;
                                await DisplayAlert("Reservation Canceled", "Your reservation was successfully canceled", "Ok");
                                RefreshSeating();
                                return;
                            }
                            else
                            {  //seat exists but not reserved
                                await DisplayAlert("Error", "Seat is not reserved.", "Ok");
                                return;
                            }
                        }
                    }
                }
                //if seat not found
                if(!correctSeat)
                    {await DisplayAlert("Error", "Seat not found.", "Ok");}
            }
        }

        //Assign to Team 3 Member
        private async void ButtonCancelReservationRange(object sender, EventArgs e)
        {
            //Sandesh Bhattarai is working in this feature
            //getting seat number from the user
            var seatRange = await DisplayPromptAsync("Cancel Seat Range", "Enter seat range (e.g., A1:A4):");
                var seats = seatRange.Split(':');
                string startSeat = seats[0].Trim();
                string endSeat = seats[1].Trim();
    
                int startRow = -1, startCol = -1, endRow = -1, endCol = -1;

                for (int i = 0; i < seatingChart.GetLength(0); i++)
                {
                    for (int j = 0; j < seatingChart.GetLength(1); j++)
                    {
                        if (seatingChart[i, j].Name == startSeat)
                        {
                            startRow = i;
                            startCol = j;
                        }
                        if (seatingChart[i, j].Name == endSeat)
                        {
                            endRow = i;
                            endCol = j;
                        }
                    }
                }
                if (startRow == -1 || startCol == -1 || endRow == -1 || endCol == -1)
                {
                    await DisplayAlert("Error", "One or more seats not found.", "Ok");
                    return;
                }
                
                if (startRow != endRow)
                {
                    await DisplayAlert("Error", "Seats must be in the same row.", "Ok");
                    return;
                }
                
                if (startCol > endCol)
                {
                    await DisplayAlert("Error", "Invalid seat range order.", "Ok");
                    return;
                }
                
                // Check if all seats in range are reserved
                for (int j = startCol; j <= endCol; j++)
                {
                    if (!seatingChart[startRow, j].Reserved)
                    {
                        await DisplayAlert("Error", "One or more seats are not reserved.", "Ok");
                        return;
                    }
                }
                for (int j = startCol; j <= endCol; j++)
                {
                    seatingChart[startRow, j].Reserved = false;
                }
                
                await DisplayAlert("Success", "Reservation canceled for selected seats.", "Ok");
                RefreshSeating();

        }
        // Implemented by Pappu Jha
        private async void ButtonResetSeatingChart(object sender, EventArgs e)
        {
            bool anySeatReserved = false;
            for (int j = 0; j < seatingChart.GetLength(0); j++)
            {
                for (int k = 0; k < seatingChart.GetLength(1); k++)
                {
                    if (seatingChart[j, k].Reserved == true)
                    {
                        anySeatReserved = true;
                        break;
                    }
                }
                if (anySeatReserved == true)
                {
                    break;
                }   
            }
            if (anySeatReserved == false)
            {
                await DisplayAlert("ALREADY RESET!!!", "All seat reservations have already been reset.", "OK");
                return;
            }

            bool askReset = await DisplayAlert("Confirm RESET Option", "Sure you want to reset all reservations?", "YES", "NO");

            if (askReset == true)
            {
                int k = 0;
                while (k < seatingChart.GetLength(0))
                {
                    int j = 0;
                    while (j < seatingChart.GetLength(1))
                    {
                        seatingChart[k, j].Reserved = false;
                        j++;
                    }
                    k++;
                }
                RefreshSeating();
                await DisplayAlert("Reset Successful :)!", "Good Job! All seat reservations have been reset.", "DONE");
            }
        }
    }
