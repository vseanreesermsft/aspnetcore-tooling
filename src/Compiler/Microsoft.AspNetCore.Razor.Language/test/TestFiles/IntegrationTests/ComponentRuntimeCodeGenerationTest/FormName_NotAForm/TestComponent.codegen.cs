﻿// <auto-generated/>
#pragma warning disable 1591
namespace Test
{
    #line default
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Threading.Tasks;
    using global::Microsoft.AspNetCore.Components;
#nullable restore
#line 1 "x:\dir\subdir\Test\TestComponent.cshtml"
using Microsoft.AspNetCore.Components.Web;

#line default
#line hidden
#nullable disable
    public partial class TestComponent : global::Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(global::Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
            __builder.OpenElement(0, "div");
            __builder.AddAttribute(1, "method", "post");
            __builder.AddAttribute(2, "onsubmit", global::Microsoft.AspNetCore.Components.EventCallback.Factory.Create<global::System.EventArgs>(this, 
#nullable restore
#line 2 "x:\dir\subdir\Test\TestComponent.cshtml"
                              () => { }

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(3, "@formname", "named-form-handler");
            __builder.CloseElement();
            __builder.AddMarkupContent(4, "\r\n");
            __builder.OpenElement(5, "div");
            __builder.AddAttribute(6, "method", "post");
            __builder.AddAttribute(7, "onsubmit", global::Microsoft.AspNetCore.Components.EventCallback.Factory.Create<global::System.EventArgs>(this, 
#nullable restore
#line 3 "x:\dir\subdir\Test\TestComponent.cshtml"
                              () => { }

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(8, "@formname", 
#nullable restore
#line 3 "x:\dir\subdir\Test\TestComponent.cshtml"
                                                      "named-form-handler"

#line default
#line hidden
#nullable disable
            );
            __builder.CloseElement();
        }
        #pragma warning restore 1998
    }
}
#pragma warning restore 1591
