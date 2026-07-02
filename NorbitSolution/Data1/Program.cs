using System.Text;

string CompoundInterest(double initial_deposit, double years, double interestRate) 
{
    StringBuilder sb = new StringBuilder();

    for (int i = 0; i < years; i++)
    {
        initial_deposit *= 1 + interestRate / 100;
        sb.Append($"Год {i + 1}: {Math.Round(initial_deposit, 2)}.\n");
    }

    string resultStr = sb.ToString();
    return resultStr;
}