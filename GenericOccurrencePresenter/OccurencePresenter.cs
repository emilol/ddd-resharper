using System.Drawing;
using JetBrains.ReSharper.Feature.Services.Occurences;
using JetBrains.ReSharper.Psi;
using JetBrains.UI.PopupMenu;
using JetBrains.UI.RichText;

namespace GenericOccurrencePresenter
{
    [OccurencePresenter]
    public class OccurrencePresenter : DeclaredElementOccurencePresenter
    {
        public override bool IsApplicable(IOccurence occurence)
        {
            return true;
        }

        protected override void DisplayMainText(IMenuItemDescriptor descriptor, 
            IOccurence occurence,
            OccurencePresentationOptions options, 
            IDeclaredElement declaredElement)
        {
            base.DisplayMainText(descriptor, occurence, options, declaredElement);

            var text = TypeParameterUtil.GetTypeText(occurence);
            if (text.Length <= 0) return;
            
            var richText = new RichText(text, TextStyle.FromForeColor(Color.DarkOliveGreen));

            richText.Prepend(descriptor.Text);

            descriptor.Text = richText;
        }
    }
}