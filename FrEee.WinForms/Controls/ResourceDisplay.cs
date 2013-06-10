using System;
using System.Drawing;
using System.Windows.Forms;
using FrEee.Utility;
using FrEee.Utility.Extensions;

namespace FrEee.WinForms.Controls
{
	public partial class ResourceDisplay : UserControl
	{
		public ResourceDisplay()
		{
			InitializeComponent();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (Resource != null)
			{
				lblAmount.ForeColor = ResourceColor;
				if (ResourceIcon != null)
					picIcon.Image = ResourceIcon;
			}
			else
			{
				lblAmount.ForeColor = Color.White;
				picIcon.Image = null;
			}
			lblAmount.Text = Amount.ToUnitString();
			if (Change != null)
			{
				lblAmount.Text += " (";
				if (Change.Value >= 0)
					lblAmount.Text += "+";
				lblAmount.Text += Change.Value.ToUnitString();
				lblAmount.Text += ")";
			}
			base.OnPaint(e);
		}

		private Resource resource;
		public Resource Resource
		{
			get { return resource; }
			set
			{
				resource = value;
				Invalidate();
			}
		}

		public string ResourceName
		{
			get { return Resource == null ? null : Resource.Name; }
			set { Resource = Resource.Find(value); }
		}

		public Color ResourceColor
		{
			get
			{
				return Resource == null ? Color.White : Resource.Color;
			}
		}

		public Image ResourceIcon
		{
			get
			{
				try
				{
					return Resource == null ? null : Resource.Icon;
				}
				catch (NullReferenceException ex)
				{
					// HACK - stupid forms designer thinks it's null and not null at the same time, WTF?!
					return null;
				}
			}
		}

		private int amount;
		public int Amount { get { return amount; } set { amount = value; Invalidate(); } }

		private int? change;
		public int? Change { get { return change; } set { change = value; Invalidate(); } }
	}
}
