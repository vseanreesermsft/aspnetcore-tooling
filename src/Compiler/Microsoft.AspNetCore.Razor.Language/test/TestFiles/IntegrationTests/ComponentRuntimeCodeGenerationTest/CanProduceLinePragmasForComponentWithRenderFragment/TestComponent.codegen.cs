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
    #line default
    #line hidden
    public partial class TestComponent : global::Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(global::Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
            __builder.OpenElement(0, "div");
            __builder.AddAttribute(1, "class", "row");
            __builder.OpenElement(2, "a");
            __builder.AddAttribute(3, "href", "#");
            __builder.AddAttribute(4, "@onclick", "Toggle");
            __builder.AddAttribute(5, "class", "col-12");
#nullable restore
#line (2,47)-(2,57) 24 "x:\dir\subdir\Test\TestComponent.cshtml"
__builder.AddContent(6, ActionText);

#line default
#line hidden
#nullable disable
            __builder.CloseElement();
#nullable restore
#line 3 "x:\dir\subdir\Test\TestComponent.cshtml"
   if (!Collapsed)
  {

#line default
#line hidden
#nullable disable
            __builder.OpenElement(7, "div");
            __builder.AddAttribute(8, "class", "col-12 card card-body");
#nullable restore
#line (6,8)-(6,20) 24 "x:\dir\subdir\Test\TestComponent.cshtml"
__builder.AddContent(9, ChildContent);

#line default
#line hidden
#nullable disable
            __builder.CloseElement();
#nullable restore
#line 8 "x:\dir\subdir\Test\TestComponent.cshtml"
  }

#line default
#line hidden
#nullable disable
            __builder.CloseElement();
        }
        #pragma warning restore 1998
#nullable restore
#line 11 "x:\dir\subdir\Test\TestComponent.cshtml"
 
  [Parameter]
  public RenderFragment ChildContent { get; set; } = (context) => 

#line default
#line hidden
#nullable disable
        __builder.OpenElement(10, "p");
#nullable restore
#line (13,71)-(13,78) 25 "x:\dir\subdir\Test\TestComponent.cshtml"
__builder.AddContent(11, context);

#line default
#line hidden
#nullable disable
        __builder.CloseElement();
#nullable restore
#line 14 "x:\dir\subdir\Test\TestComponent.cshtml"
  [Parameter]
  public bool Collapsed { get; set; }
  string ActionText { get => Collapsed ? "Expand" : "Collapse"; }
  void Toggle()
  {
    Collapsed = !Collapsed;
  }

#line default
#line hidden
#nullable disable
    }
}
#pragma warning restore 1591
