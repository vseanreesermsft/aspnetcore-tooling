// <auto-generated/>
#pragma warning disable 1591
namespace Test
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
    public class TestComponent : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.RenderTree.RenderTreeBuilder builder)
        {
            __Blazor.Test.TestComponent.TypeInference.CreateMyComponent_0(builder, 0, 1, "hi", 2, (context) => (builder2) => {
                builder2.AddMarkupContent(3, "\r\n  ");
                builder2.OpenElement(4, "div");
                builder2.AddContent(5, context.ToLower());
                builder2.CloseElement();
                builder2.AddMarkupContent(6, "\r\n");
            }
            );
        }
        #pragma warning restore 1998
    }
}
namespace __Blazor.Test.TestComponent
{
    #line hidden
    internal static class TypeInference
    {
        public static void CreateMyComponent_0<TItem>(global::Microsoft.AspNetCore.Components.RenderTree.RenderTreeBuilder builder, int seq, int __seq0, TItem __arg0, int __seq1, global::Microsoft.AspNetCore.Components.RenderFragment<TItem> __arg1)
        {
        builder.OpenComponent<global::Test.MyComponent<TItem>>(seq);
        builder.AddAttribute(__seq0, "Item", __arg0);
        builder.AddAttribute(__seq1, "ChildContent", __arg1);
        builder.CloseComponent();
        }
    }
}
#pragma warning restore 1591
