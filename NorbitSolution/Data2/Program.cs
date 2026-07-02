void DrowRhombus(int N)
{
    for (int i = 0; i < N; i++)
    {
        for (int j = 0; j < N; j++)
            if (i + N / 2 == j || i - N / 2 == j || 
                i == N / 2 - j || i == N / 2 + N - j - 1)
                Console.Write("X");
            else
                Console.Write(" ");
        Console.Write("\n");
    }
}