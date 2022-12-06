using ModelBuilder.Builders;

namespace App
{
    internal class MercadoLivreProdutoDescriptor : ModelDescriptorBuilder<Produto>
    {
        public MercadoLivreProdutoDescriptor()
        {
            this.ForMember(x => x.Titulo)
                .QuerySelect(".ui-pdp-title")
                .ExtractFromInnerHtml()
            .Build()
            .ForMember(x => x.Preco)
                .QuerySelect("div.ui-pdp-price__second-line span.andes-money-amount__fraction")
                .ExtractFromInnerHtml()
                .ConcatWith(concatB => concatB
                    .QuerySelect("span.andes-money-amount__cents.andes-money-amount__cents--superscript-36")
                    .ExtractFromInnerHtml()
                    .HasDefaultValue(0)
                    .AddStringFilter(static f => $",{f}")
                    )
            .Build()
            

            .ForManyMembers(x => x.Comentarios)
                .QuerySelect(".ui-review-capability-comments__comment__content")
                .ExtractFromInnerHtml()
                .AllowNullValue()
            .Build()

            .ForComplexMember(x => x.PrecoObjt)
                .ForMember(x => x.Real)
                .QuerySelect("div.ui-pdp-price__second-line span.andes-money-amount__fraction")
                .ExtractFromInnerHtml()
                .Build()
            .ForMember(x => x.Centavos)
                .QuerySelect("span.andes-money-amount__cents.andes-money-amount__cents--superscript-36")
                .ExtractFromInnerHtml()
                .HasDefaultValue(0)
                .Build()
            .Build()


            .ForMember(x => x.Vendido)
                .QuerySelect("span.ui-pdp-subtitle")
                .AddStringFilter(x => x == "Usado" ? "0" : x == "Novo" ? "0" : x)
                .AddStringFilter(StringFilters.OnlyNumbers)
                .ExtractFromInnerHtml()
                .Build()
            .ForMember(x => x.Vendido)
                .QuerySelect(".andes-badge__content")
                .AddStringFilter(x => x == "Recondicionado" ? "0" : null)
                .AddStringFilter(StringFilters.OnlyNumbers)
                .ExtractFromInnerHtml()
                .Build()
            
                
            .ForMember(x => x.Estoque)
                .QuerySelect("span.ui-pdp-buybox__quantity__available")
                .ExtractFromInnerText()
                .AddPosValidator(x => x >= 0)
                .AddStringFilter(StringFilters.OnlyNumbers)
                .Build()
            .ForMember(x => x.Estoque)
                .QuerySelect("div.ui-pdp-buybox__quantity p")
                .AddPosValidator(x => x >= 0)
                .ExtractFromInnerText()
                .AddStringFilter(x => x.ToLower().Contains("Último disponível!".ToLower()) ? "1" : null)
                .AddStringFilter(StringFilters.OnlyNumbers)
                .Build()
            .ForMember(x => x.Estoque)
                .QuerySelect("span.ui-pdp-buybox__quantity__selected")
                .AddPosValidator(x => x >= 0)
                .AddStringFilter(StringFilters.OnlyNumbers)
                .ExtractFromInnerText()
                .Build()
            
            
            .ForMember(x => x.Descricao)
                .QuerySelect("p.ui-pdp-description__content")
                .HasDefaultValue("")
                .ExtractFromInnerText()
                .Build()
            .ForMember(x => x.Foto)
                .QuerySelect(".ui-pdp-gallery__figure img")
                .HasDefaultValue("")
                .ExtractFromInnerText()
                .Build()
            .ForMember(x => x.IdExterno)
                .QuerySelect("input[name=\"item_id\"]")
                .ExtractFromAttribute("value")
                .DisallowNullValue()
                .Build()
            .ForMember(x => x.IsFulFillment)
                .QuerySelect("span.ui-pdp-media__title-icons .ui-pdp-icon.ui-pdp-icon--full")
                .AddStringFilter(x => x == null ? "false" : "true")
                .ExtractFromInnerHtml()
                .Build()
            .ForMember(x => x.IsCatalog)
                .QuerySelect(".ui-pdp-other-sellers")
                .AddStringFilter(x => x == null ? "false" : "true")
                .ExtractFromInnerHtml()
                .Build()
            .Build();

        }
    }
}
