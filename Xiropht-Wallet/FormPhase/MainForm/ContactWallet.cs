using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Xiropht_Wallet.FormPhase.ParallelForm;

namespace Xiropht_Wallet.FormPhase.MainForm
{
    public partial class ContactWallet : Form
    {
        private bool contactLoaded;

        public ContactWallet()
        {
            InitializeComponent();
        }

        private void buttonAddContact_Click(object sender, EventArgs e)
        {
            var addContactForm = new AddContactWallet();
            addContactForm.ShowDialog(ClassFormPhase.WalletXiropht);
            addContactForm.Dispose();
        }

        public void GetListControl()
        {
            if (ClassFormPhase.WalletXiropht.ListControlSizeContactWallet.Count == 0)
            {
                for (int i = 0; i < Controls.Count; i++)
                {
                    if (i < Controls.Count)
                    {
                        ClassFormPhase.WalletXiropht.ListControlSizeContactWallet.Add(
                            new Tuple<Size, Point>(Controls[i].Size, Controls[i].Location));
                    }
                }
            }
        }

        private void ContactWallet_Load(object sender, EventArgs e)
        {
            UpdateStyles();
            ClassFormPhase.WalletXiropht.ResizeWalletInterface();
            if (!contactLoaded) // Load contact database file.
            {
                contactLoaded = true;
                if (ClassContact.ListContactWallet.Count > 0)
                {
                    foreach (var contact in ClassContact.ListContactWallet)
                    {
                        string[] objectContact = { contact.Key, contact.Value, "X" };
                        ListViewItem itemContact = new ListViewItem(objectContact);
                        listViewExContact.Items.Add(itemContact);
                    }
                }
            }
        }

        private void ContactWallet_Resize(object sender, EventArgs e)
        {
            UpdateStyles();
        }

        private void listViewExContact_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = listViewExContact.GetItemAt(0, e.Y);

            bool found = false;
            if (item == null) return;
            for (int ix = item.SubItems.Count - 1; ix >= 0; --ix)
                if (item.SubItems[ix].Bounds.Contains(e.Location))
                {
                    if (!found)
                    {
                        found = true;
                        if (item.SubItems[ix].Text != "X")
                        {
                            Clipboard.SetText(item.SubItems[ix].Text);
#if WINDOWS
                            new Thread(() =>
                                    ClassFormPhase.MessageBoxInterface(item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder("CONTACT_LIST_COPY_ACTION_CONTENT_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information))
                                .Start();
#else
                        new Thread(delegate ()
                        {
                            MethodInvoker invoker = () => MessageBox.Show(ClassFormPhase.WalletXiropht, item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder("CONTACT_LIST_COPY_ACTION_CONTENT_TEXT"));
                            BeginInvoke(invoker);
                        }).Start();
#endif
                        }
                        else
                        {
                            ClassContact.RemoveContact(item.SubItems[0].Text); // Remove contact by his name.
                            listViewExContact.Items.Remove(item);
                            listViewExContact.Refresh();
                        }
                        return;
                    }
                }
        }
    }
}
