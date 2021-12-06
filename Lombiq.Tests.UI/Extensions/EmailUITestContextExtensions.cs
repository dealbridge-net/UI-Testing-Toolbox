using Atata;
using Lombiq.Tests.UI.Helpers;
using Lombiq.Tests.UI.Services;
using OpenQA.Selenium;
using System;

namespace Lombiq.Tests.UI.Extensions
{
    public static class EmailUITestContextExtensions
    {
        /// <summary>
        /// Navigates to the smtp4dev web UI that is launched if <see
        /// cref="OrchardCoreUITestExecutorConfiguration.UseSmtpService"/> is set to <see langword="true"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the smtp4dev server is not running.</exception>
        public static void GoToSmtpWebUI(this UITestContext context)
        {
            if (context.SmtpServiceRunningContext == null)
            {
                throw new InvalidOperationException(
                    "The SMTP service is not running. Did you turn it on with " +
                    nameof(OrchardCoreUITestExecutorConfiguration) + "." + nameof(OrchardCoreUITestExecutorConfiguration.UseSmtpService) +
                    " and could it properly start?");
            }

            context.GoToAbsoluteUrl(context.SmtpServiceRunningContext.WebUIUri);
        }

        /// <summary>
        /// Finds and leaves open the first email in the smtp4dev Web UI whose title contains <paramref
        /// name="emailTitle"/> and message body contains <paramref name="textToFind"/>. If none are found <see
        /// cref="NotFoundException"/> is thrown.
        /// </summary>
        public static IWebElement FindSpecificEmailInInbox(
            this UITestContext context,
            string emailTitle,
            string textToFind)
        {
            context.GoToSmtpWebUI();
            context.ClickReliablyOn(ByHelper.SmtpInboxRow(emailTitle));
            context.SwitchToFrame0();

            var currentlySelectedEmail = context.Get(By.CssSelector(".emailContent p"));
            while (!currentlySelectedEmail.Text.Contains(textToFind, StringComparison.InvariantCultureIgnoreCase))
            {
                context.SwitchToFirstWindow();
                context.ClickReliablyOn(By.CssSelector(".unread").Within(TimeSpan.FromMinutes(2)));
                context.SwitchToFrame0();

                currentlySelectedEmail = context.Get(By.CssSelector(".emailContent p"));
            }

            return currentlySelectedEmail;
        }
    }
}