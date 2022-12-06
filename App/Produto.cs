
public class Produto
{
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public double Preco { get; set; }
    public int Estoque { get; set; }
    public int Vendido { get; set; }
    public bool IsCatalog { get; set; }
    public bool IsFulFillment { get; set; }
    public string IdExterno { get; set; }
    public string? Foto { get; set; }
    public Preco PrecoObjt { get; set; }

    public List<string> Comentarios { get; set; }
}

public class Preco
{
    public int Real { get; set; }
    public int Centavos { get; set; }
}