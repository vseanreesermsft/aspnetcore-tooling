﻿// <auto-generated/>
#pragma warning disable 1591
namespace Test
{
    #line default
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Threading.Tasks;
#nullable restore
#line 1 "x:\dir\subdir\Test\TestComponent.cshtml"
using Microsoft.AspNetCore.Components;

#line default
#line hidden
#nullable disable
    public partial class TestComponent<
#nullable restore
#line 2 "x:\dir\subdir\Test\TestComponent.cshtml"
TItem

#line default
#line hidden
#nullable disable
    > : global::Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 219
        private void __RazorDirectiveTokenHelpers__() {
        ((global::System.Action)(() => {
        }
        ))();
        }
        #pragma warning restore 219
        #pragma warning disable 0414
        private static object __o = null;
        #pragma warning restore 0414
        #pragma warning disable 1998
        protected override void BuildRenderTree(global::Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
#nullable restore
#line 6 "x:\dir\subdir\Test\TestComponent.cshtml"
__o = ChildContent(Items1);

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "x:\dir\subdir\Test\TestComponent.cshtml"
 foreach (var item in Items2)
{
    

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "x:\dir\subdir\Test\TestComponent.cshtml"
  __o = ChildContent(item);

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "x:\dir\subdir\Test\TestComponent.cshtml"
                              
}

#line default
#line hidden
#nullable disable
#nullable restore
#line 13 "x:\dir\subdir\Test\TestComponent.cshtml"
__o = ChildContent(Items3());

#line default
#line hidden
#nullable disable
        }
        #pragma warning restore 1998
#nullable restore
#line 15 "x:\dir\subdir\Test\TestComponent.cshtml"
       
    [Parameter] public TItem[] Items1 { get; set; }
    [Parameter] public List<TItem[]> Items2 { get; set; }
    [Parameter] public Func<TItem[]> Items3 { get; set; }
    [Parameter] public RenderFragment<TItem[]> ChildContent { get; set; }

#line default
#line hidden
#nullable disable
    }
}
#pragma warning restore 1591
