using ModelBuilder.Extensions;
using ModelBuilder.Models;
using System.Linq.Expressions;


namespace ModelBuilder.Builders;


public class ModelDescriptorBuilder<TEntity> :
    ModelDescriptorBuilder<TEntity,ModelDescription>
{
    public ModelDescriptorBuilder():base()
    {
        BuildValue = this;
    }
}


public class ModelDescriptorBuilder<TEntity, TBuild> : 
    BaseModelDescriptorBuilder<TEntity,ModelDescriptorBuilder<TEntity,TBuild>>
{
    public ModelDescriptorBuilder(TBuild buildValue):this()
    {
        BuildValue = buildValue;
    }
    public ModelDescriptorBuilder()
    {
        base.self = this;
    }

    protected TBuild BuildValue;

    public TBuild Build() => BuildValue;
}


public class BaseModelDescriptorBuilder<TEntity,TContext>  : ModelDescription
    where TContext : BaseModelDescriptorBuilder<TEntity, TContext> 
{

    protected TContext self;

    public MemberDescriptionBuilder<TEntity, TField, TContext> ForManyMembers<TField>(Expression<Func<TEntity, IEnumerable<TField>>> fieldSelector)
    {
        var member = new MemberDescriptionBuilder<TEntity, TField, TContext>(self, true, fieldSelector.GetMemberName());
        this.AddMemberDescription(member);
        return member;
    }

    public MemberDescriptionBuilder<TEntity, TField, TContext> ForMember<TField>(Expression<Func<TEntity, TField>> fieldSelector)
    {
        var isList = typeof(TField).IsList();
        var member = new MemberDescriptionBuilder<TEntity, TField, TContext>(self, isList, fieldSelector.GetMemberName());
        this.AddMemberDescription(member);
        return member;
    }

    public ModelDescriptorBuilder<TField, TContext> ForComplexMember<TField>(Expression<Func<TEntity, TField>> fieldSelector)
        where TField : class
    {
        var memberName = fieldSelector.GetMemberName();
        var descriptor = new ModelDescriptorBuilder<TField, TContext>(self);
        AddComplexMember(memberName, descriptor);

        return descriptor;
    }

    public BaseModelDescriptorBuilder<TEntity, TContext> InContainer(string containerSelector)
    {
        this.ContainerSelector = containerSelector;
        return this;
    }


    private void AddMemberDescription(MemberDescription memberDescription)
    {
        if (!this.members.ContainsKey(memberDescription.ValueDescription.MemberName))
            this.members.Add(memberDescription.ValueDescription.MemberName, new List<MemberDescription>());
        this.members[memberDescription.ValueDescription.MemberName].Add(memberDescription);
    }

    private void AddComplexMember(string memberName, ModelDescription descriptor)
    {
        if (!this.complex_members.ContainsKey(memberName))
            this.complex_members[memberName] = new();
        this.complex_members[memberName].Add(descriptor);
    }


   
}

