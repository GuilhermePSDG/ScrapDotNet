using ModelBuilder.Models;

namespace ModelBuilder.Builders;

public class MemberDescriptionBuilder<TEntity, TField, TBuild> :
    MemberDescription
{

    private TBuild BuildValue { get; }

    public MemberDescriptionBuilder(TBuild BuildValue, bool IsCollection, string MemberName)
    {
        var fieldType = typeof(TField);
        if (IsCollection)
            fieldType = fieldType.GenericTypeArguments.FirstOrDefault() ?? fieldType;
        if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
            fieldType = fieldType.GenericTypeArguments.FirstOrDefault() ?? fieldType;


        this.ValueDescription.IsCollection = IsCollection;
        this.ValueDescription.MemberType = ObjectParser.ParseTypeToObjectType(fieldType);
        this.ValueDescription.MemberName = MemberName;
        this.BuildValue = BuildValue;
    }



    #region Selection
    public MemberDescriptionBuilder<TEntity, TField, TBuild> QuerySelect(string querySelector)
    {
        this.QuerybleDescription.QuerySelector = querySelector;
        return this;
    }
    public MemberDescriptionBuilder<TEntity, TField, TBuild> InContainer(string querySelector)
    {
        this.QuerybleDescription.ContainerSelector = querySelector;
        return this;
    }
    #endregion

    #region Filter
    //public MemberDescriptionBuilder<TEntity, TField, TBuild> UseDefaultStringFilters()
    //{
    //    return this.UseStringFilter(this.ValueDescription.MemberType switch
    //    {
    //        ObjectType.String => ITextCleaner.CreateBuilder().Build().Clean,
    //        ObjectType.DateTime => ITextCleaner.CreateBuilder().AllowOnlyDateTime().Build().Clean,
    //        ObjectType.Bool => ITextCleaner.CreateBuilder().AllowOnlyBoolean().Build().Clean,
    //        ObjectType.Guid => ITextCleaner.CreateBuilder().AllowOnlyGuid().Build().Clean,
    //        ObjectType.TimeSpan => ITextCleaner.CreateBuilder().AllowOnlyTimeSpan().Build().Clean,
    //        ObjectType.DateTimeOffset => ITextCleaner.CreateBuilder().AllowOnlyDateOffset().Build().Clean,
    //        ObjectType.Uri => ITextCleaner.CreateBuilder().AllowOnlyUri().Build().Clean,
    //        ObjectType.Char => ITextCleaner.CreateBuilder().AllowOnlyOneChar().Build().Clean,
    //        ObjectType.Int => ITextCleaner.CreateBuilder().AllowOnlyNumbers().Build().Clean,
    //        ObjectType.Byte => ITextCleaner.CreateBuilder().AllowOnlyNumbers().Build().Clean,
    //        ObjectType.Sbyte => ITextCleaner.CreateBuilder().AllowOnlyNumbers().Build().Clean,
    //        ObjectType.Short => ITextCleaner.CreateBuilder().AllowOnlyNumbers().Build().Clean,
    //        ObjectType.Ushort => ITextCleaner.CreateBuilder().AllowOnlyNumbers().Build().Clean,
    //        ObjectType.Uint => ITextCleaner.CreateBuilder().AllowOnlyNumbers().Build().Clean,
    //        ObjectType.Long => ITextCleaner.CreateBuilder().AllowOnlyNumbers().Build().Clean,
    //        ObjectType.Ulong => ITextCleaner.CreateBuilder().AllowOnlyNumbers().Build().Clean,
    //        ObjectType.Float => ITextCleaner.CreateBuilder().AllowOnlyDecimal().Build().Clean,
    //        ObjectType.Double => ITextCleaner.CreateBuilder().AllowOnlyDecimal().Build().Clean,
    //        ObjectType.Decimal => ITextCleaner.CreateBuilder().AllowOnlyDecimal().Build().Clean,
    //        _ => throw new InvalidOperationException($"Type \"{this.ValueDescription.MemberType}\" is not supported"),
    //    });
    //}

    public MemberDescriptionBuilder<TEntity, TField, TBuild> AddStringFilter(Func<string, string> cleaner)
    {
        this.Options.Cleaner.Add(cleaner);
        return this;
    }

    #endregion

    #region Nulable
    public MemberDescriptionBuilder<TEntity, TField, TBuild> AllowNullValue()
    {
        this.Options.AllowNull = true;
        return this;
    }
    public MemberDescriptionBuilder<TEntity, TField, TBuild> DisallowNullValue()
    {
        this.Options.AllowNull = false;
        return this;
    }
    #endregion

    #region Extraction
    public MemberDescriptionBuilder<TEntity, TField, TBuild> ExtractFromInnerHtml() => this.ExtractFrom(MemberValueLocation.InnerHtml);
    public MemberDescriptionBuilder<TEntity, TField, TBuild> ExtractFromInnerText() => this.ExtractFrom(MemberValueLocation.InnerText);
    public MemberDescriptionBuilder<TEntity, TField, TBuild> ExtractFromAttribute(string AtributeName) => this.ExtractFrom(MemberValueLocation.Attribute, AtributeName);
    public MemberDescriptionBuilder<TEntity, TField, TBuild> ExtractFrom(MemberValueLocation memberValueLocation, string? Value = null)
    {
        this.QuerybleDescription.Location = memberValueLocation;
        this.QuerybleDescription.LocationValue = Value;
        return this;
    }
    #endregion

    #region Validator
    public MemberDescriptionBuilder<TEntity, TField, TBuild> AddValidator(Func<string, bool> validator)
    {
        this.Options.Validators.Add(validator);
        return this;
    }
    public MemberDescriptionBuilder<TEntity, TField, TBuild> AddPosValidator(Func<TField, bool> validator)
    {
        this.Options.PosValidators.Add((x) => validator((TField)x));
        return this;
    }
    #endregion


    public MemberDescriptionBuilder<TEntity, TField, TBuild> ConcatWith(Action<MemberDescriptionBuilder<TEntity, TField, MemberDescriptionBuilder<TEntity, TField, TBuild>>> ConcatBuilder)
    {
        var builder = new MemberDescriptionBuilder<TEntity, TField, MemberDescriptionBuilder<TEntity, TField, TBuild>>(this, false, "concat_member");
        ConcatBuilder(builder);
        this.ConcatMember = builder;
        return this;
    }


    public MemberDescriptionBuilder<TEntity, TField, TBuild> HasDefaultValue(TField value)
    {
        this.Options.DefaultValue = value;
        return this;
    }

    public TBuild Build() => this.BuildValue;
}
    

