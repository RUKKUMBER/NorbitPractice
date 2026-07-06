DiscountedProduct product = new DiscountedProduct();
Console.WriteLine(product.ToString());

public class DiscountedProduct
{
    private string _name { get; }
    private string _fabricator { get; }
    private double _price { get; }
    private string _expirationDate { get; }
    private string _productionDate { get; }
    private double _discount { get; }
    private double _discountPrice { get; }

    public DiscountedProduct()
    {
        Console.WriteLine("Введите название товара");
        _name = Console.ReadLine();
        Console.WriteLine("Введите производителя");
        _fabricator = Console.ReadLine();
        Console.WriteLine("Введите цену");
        _price = double.Parse(Console.ReadLine());
        Console.WriteLine("Введите срок годности");
        _expirationDate = Console.ReadLine();
        Console.WriteLine("Введите дату производства");
        _productionDate = Console.ReadLine();
        Console.WriteLine("Введите размер скидки");
        _discount = double.Parse(Console.ReadLine());
        _discountPrice = _price * (1 -  _discount / 100);
    }

    public override string ToString()
    {
        string str = $"Название товара: {_name}, производитель: {_fabricator}, срок годности: {_expirationDate}, " +
            $"дата производства: {_productionDate}, цена: {_price}, размер скидки: {_discount}, акционная цена: {_discountPrice}";

        return str;
    }
}