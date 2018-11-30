
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO
using System.Collections;


namespace DaCheekyCow
{
    

    
    public partial class Form1 : Form
    {
        //declaration of varaibale to be used in the program to get the index of the meal
        //selected and the size from the listbox
        int GetMealSizeIndex;
        int GetMealTypeIndex;                                             //access keys, tooltips, price per line item and
                                                                    //proper naming convention,design and comment
        //reads all text from the price text file into the pricefile
        String PriceFile = File.ReadAllText(@"Price.txt");

        //declaration of variables to be used in looping in the pricelist 
        int a = 0, b = 0;

        //declaration of a 2d array with size 5x12
        decimal[,] PriceList2DArray = new decimal[5, 12];
        //used to parse user input(quantity entered) 
        int UserInputQuantity;
        //-----------------------------------------------------------

        //reads all text in stocks.txt to stockfile
        String StockFile = File.ReadAllText(@"stocks.txt");
        string StockFileOutput= @"stocks.txt";

        int c = 0, d = 0;
        //declartion of a 2d array for stocks with size 5*12
        int[,] StockList2DArray = new int[5, 12];
        //----------------------------------------------------------
        //
        decimal Cost;
        int AvailableStock;
        //-------------------------------------------------------
        List<string> MealTypeChoosen = new List<string>();
        List<string> MealSizeChoosen = new List<string>();
        List<int> QuantityChoosen = new List<int>();
        List<decimal> PriceOfChoice = new List<decimal>();
        //-----------------------------------------------------
        string Meals, Sizes, Quantity, Amount;
        decimal Total;

        //-------------------------------------------------------
        //used to store a formatted message when a message box pops up
        string Message;
        //----------------------------------------------
        
        //streamwriter declartion, would be used to write the updated stock array in memory to file
        //when the program exits 
        StreamWriter UpdateStockFile;
        //=======
        int Id;
        //=============
        string TransactionNumber;
        //
        decimal RunningTotal;
        //
        //Random number generator for the Id of each transaction
        const int SPACE = 6;
        int[] TransactId = new int[SPACE];
        Random GenerateRandom = new Random();


        public Form1()
        {
            InitializeComponent();
        }
        //event handler for when the form loads
        private void Form1_Load(object sender, EventArgs e)
        {

            CompleteOrderButton.Enabled = false;
            ClearButton.Enabled = false;

            //setting acces keys 
            AddToOrderButton.Text = "&Add to Order";
            CompleteOrderButton.Text = "&Complete order";
            ClearButton.Text = "& Clear";
            ExitButton.Text = "&Exit";
            LiveStockButton.Text = "&Live Stock";


            //loops from the stock file into the array that would hold the stocks
            foreach (var Row in StockFile.TrimEnd().Split('\n'))
            {
                d = 0;
                foreach (var Col in Row.Trim().Split(' '))
                {
                    
                    StockList2DArray[c, d] = int.Parse(Col.Trim());
                    d++;

                }
                c++;

            }


            //loops from the price list into the array that would hold the price
            foreach (var Row in PriceFile.Split('\n'))
            {
                b = 0;
                foreach (var Col in Row.Trim().Split(' '))
                {
                    PriceList2DArray[a, b] = decimal.Parse(Col.Trim());

                    b++;

                }
                a++;

            }
        }
        //add to order button event handling
        private void AddToOrderButton_Click(object sender, EventArgs e)
        {
            //declaration 
            int Identify;


            //gets the available stock for the choosen option 
            AvailableStock = (StockList2DArray[GetMealSizeIndex, GetMealTypeIndex]);

            try
            {  //tries to parse user input to an integer
                UserInputQuantity = int.Parse(UserInputTextBox.Text);
                if (UserInputQuantity > 0)//checks if the input is greater than zero
                {
                    //if what the user enters is equal to or less than the avalable stock
                    if (UserInputQuantity <= AvailableStock)
                    {
                        DisplayPrice.Text = (UserInputQuantity * Cost).ToString();

                        MealTypeChoosen.Add((MealTypeListBox.SelectedItem).ToString());
                        MealSizeChoosen.Add((MealSizeListBox.SelectedItem).ToString());
                        QuantityChoosen.Add(UserInputQuantity);
                        PriceOfChoice.Add(decimal.Parse(DisplayPrice.Text));



                        RunningTotal += UserInputQuantity * Cost;



                        TotalPriceLabel.Text= RunningTotal.ToString();

                        CompleteOrderButton.Enabled = true;
                        ClearButton.Enabled = true;

                        //-------------------------------------
                        for (Identify = MealTypeChoosen.Count - 1; Identify != MealTypeChoosen.Count; Identify++)
                        {
                            OrderListBox.Items.Add(MealTypeChoosen.ElementAt(Identify) + "\t" + MealSizeChoosen.ElementAt(Identify) +
                           "\t" + QuantityChoosen.ElementAt(Identify) + "\t"
                           +Cost.ToString()+"\t"+ PriceOfChoice.ElementAt(Identify).ToString("C"));


                            (StockList2DArray[GetMealSizeIndex, GetMealTypeIndex]) = AvailableStock - UserInputQuantity;// this should work else i am in for it

                        }
                        //OrderListBox.ClearSelected();


                        MealTypeListBox.SelectedIndex = -1;
                        MealSizeListBox.SelectedIndex = -1;
                        DisplayPrice.Text = "";
                        UserInputTextBox.Text = "";


                    }
                    else// if the user input doesn't meet the above conditions i.e, greater than the available stock
                    {
                        MessageBox.Show("the available stock is:" + AvailableStock, "Warning",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        UserInputTextBox.Text = AvailableStock.ToString();
                        UserInputTextBox.Focus();
                        UserInputTextBox.SelectAll();
                        DisplayPrice.Text = "";

                    }

                }
                else
                {
                    //if the user input is negative or inavlid
                    MessageBox.Show("Please enter a valid quantity", "Invalid input",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    UserInputTextBox.Focus();
                    UserInputTextBox.SelectAll();


                }

            }
            catch //if user input can't be parsed
            {
                MessageBox.Show("Please enter a quantity greater than 1", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UserInputTextBox.Focus();
                UserInputTextBox.SelectAll();

            }
        }

        //event handler for the complete order button
        private void CompleteOrderButton_Click(object sender, EventArgs e)
        {

            Meals = "";
            Sizes = "";
            Total = 0;
            Quantity = "";
            Amount = "";


            foreach (string Meal in MealTypeChoosen)
            {
                Meals += (Meal) + "\t ".ToString();
            }
            
            foreach (string Size in MealSizeChoosen)
            {
                Sizes += (Size) + "\t".ToString();
            }

            for (Id = 0; Id < MealTypeChoosen.Count; Id++)
            {
                Total += PriceOfChoice.ElementAt(Id);

                Quantity = Quantity + "\t" + QuantityChoosen.ElementAt(Id);

                Amount = Amount + "\t" + PriceOfChoice.ElementAt(Id);
            }
            //---------------------------------------------


           //generates random numbers
            for (int index = 0; index <
                 TransactId.Length; index++)
            {
                TransactId[index] = GenerateRandom.Next(100);
                TransactionNumber = TransactId[1].ToString() + TransactId[2].ToString() + TransactId[3].ToString();
            }
            //--------------------------------------------

            TotalPriceLabel.Text = Total.ToString("C");

            

            //---------------------------------------
            Message = "Meal Type: " + Meals + "\n" + "Meal Size: " + Sizes + "\n"
                + "Quantity Choosen: " + Quantity + "\n" + "Prices: " + Amount + "\n" + "Total Amount due: " + Total.ToString("C");

            MessageBox.Show(Message, "Receipt", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //====================================================




        }

        //event handler for the clear button
        private void ClearButton_Click(object sender, EventArgs e)
        {


            MealTypeListBox.SelectedIndex = -1;//unselects meal selection
            MealSizeListBox.SelectedIndex = -1;//unselects size selection
            DisplayPrice.Text = "";
            UserInputTextBox.Text = "";
            OrderListBox.Items.Clear();

            RunningTotal = 0;

            //---------------------------------
            CompleteOrderButton.Enabled = false; //disables the following button
            ClearButton.Enabled = false;
            //-------------------------------
            //clears the arrays holding user selections
            MealSizeChoosen.Clear();
            MealTypeChoosen.Clear();
            QuantityChoosen.Clear();
            PriceOfChoice.Clear();
            //--------------------------------------
            //if order is cancelled return the amount deducted from stock back
            (StockList2DArray[GetMealSizeIndex, GetMealTypeIndex]) = AvailableStock + UserInputQuantity;

        }

        //event handler for the exit button
        private void ExitButton_Click(object sender, EventArgs e)
        {
            
            //warn users if the actually want to close the program, to help them recover back
            //from accidentally clicking the exit button
            DialogResult ConfirmIfUserWantsToExit= MessageBox.Show("Are Sure you want to close the program", "Warning", MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (ConfirmIfUserWantsToExit== DialogResult.Yes)//if the user selects YES
            {
                WriteToFile(@"", "Transactions.txt"); //call the write to file method
                //ArrayToText(Result,input);//update the stock text file
                this.Close();//close the program
            }
      
            
        }



       //handles events of mouse clicks, when the user clicks on a meal type
        private void MealType_MouseClick(object sender, MouseEventArgs e)
        {
            GetMealTypeIndex = MealTypeListBox.SelectedIndex;
        }


        //event handler for when a user clicks on a meal size
        private void MealSizeListBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (MealTypeListBox.SelectedIndex>-1)
            {
                GetMealSizeIndex = MealSizeListBox.SelectedIndex;

                Cost = (PriceList2DArray[GetMealSizeIndex, GetMealTypeIndex]);
                DisplayPrice.Text = Cost.ToString("C");

            }
            else
            {
                MessageBox.Show("Please select a meal type", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MealSizeListBox.SelectedIndex = -1;
                
            }
        }

        //event handler for the live stock button, clicking this button displays  the stock grid view
        private void LiveStockButton_Click(object sender, EventArgs e)
        {
            StockGridView.Rows.Clear();
            if (StockGridView.Visible == true)
            {
                StockGridView.Visible = false;
            }
            else if (StockGridView.Visible == false)
            {
                StockGridView.Visible = true;
            }
            //gets the height and weight of the stock array
            int Height = StockList2DArray.GetLength(0);
            int Width = StockList2DArray.GetLength(1);

            this.StockGridView.ColumnCount = Width;

            for (int Grid = 0; Grid < Height; Grid++)
            {
                DataGridViewRow GetRows = new DataGridViewRow();
                GetRows.CreateCells(this.StockGridView);

                for (int View = 0; View < Width; View++)
                {
                    GetRows.Cells[View].Value = StockList2DArray[Grid, View];
                }

                this.StockGridView.Rows.Add(GetRows);
            }

        }
                 
        //method for writing to file
        public void WriteToFile(string WriteDirectory, string name)
        {
            string NameOfFile = String.Format("{0:dd-MM-yyyy}_of_{1}", DateTime.Now, name);
            string MyPreferedLocation = Path.Combine(WriteDirectory, NameOfFile);
            using (StreamWriter Writer = File.AppendText(MyPreferedLocation))
            
            {
                Writer.WriteLine("ID: " + TransactionNumber);
                Writer.WriteLine("Meal type: " + Meals);
                Writer.WriteLine("Meal Size: " + Sizes);
                Writer.WriteLine("Quantity: " + Quantity);
                Writer.WriteLine("Price: " + Amount);
                Writer.WriteLine("Total Cost: " + Total);
                Writer.WriteLine("----------------------------------------");
                Writer.Close();
            }

        }
        //writes back the array to a textfile
        public void ArrayToText(Array ArrayOfTheStock, string FileToWrite)
        {
            int Q, R;

            

            System.IO.File.WriteAllText(StockFileOutput, string.Empty);
            UpdateStockFile = File.AppendText(StockFileOutput);
            for(Q=0; Q<5; Q++)
            {
                for(R=0; R<12; R++)
                {
                    UpdateStockFile.Write(StockList2DArray[Q, R].ToString() + " ");
                }
                UpdateStockFile.WriteLine("\n");
            }
            UpdateStockFile.Close();
        }

                        

}
}

