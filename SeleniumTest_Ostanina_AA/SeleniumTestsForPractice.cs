using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace SeleniumTest_Ostanina_AA;

public class SeleniumTestsForPractice
{
    private const string login = "Siama16@mail.ru";
    private const string password = "Tinctura92!";
    private const string baseUrl = "https://staff-testing.testkontur.ru/";
    private static readonly TimeSpan defaultTimeout = TimeSpan.FromSeconds(3);

    private ChromeDriver driver;
    private WebDriverWait waiter;

    [SetUp]
    public void Setup()
    {
        var options = new ChromeOptions
        {
            ImplicitWaitTimeout = defaultTimeout,
            PageLoadTimeout = defaultTimeout,
            ScriptTimeout = defaultTimeout
        };
        options.AddArguments("--no-sandbox", "--disable-extensions", "--window-size=1200,1200");

        driver = new ChromeDriver(options);
        waiter = new WebDriverWait(driver, defaultTimeout);
        Authorization();
    }

    [TearDown]
    public void TearDown()
    {
        driver.Quit();
    }

    [Test]
    public void Should_navigate()
    {
        OpenSideMenu();
        
        var community = driver.FindElements(By.CssSelector("[data-tid='Community']"))
            .First(element => element.Displayed);
        community.Click();
        var communityTitle = driver.FindElement(By.CssSelector("[data-tid='Title']"));
        
        driver.Url.Should().Be($"{baseUrl}communities");
    }

    [Test]
    public void Should_create_folder()
    {
        OpenSideMenu();
        
        var files = driver.FindElements(By.CssSelector("[data-tid='Files']"))
            .First(element => element.Displayed);
        files.Click();
        ClickOnElementByCssSelector("section [data-tid='DropdownButton']");
        ClickOnElementByCssSelector("[data-tid='CreateFolder']");
        var folderNameInput = driver.FindElement(By.CssSelector("[data-tid='Input']"));
        var folderName = GenerateGuidString();
        folderNameInput.SendKeys(folderName);
        ClickOnElementByCssSelector("[data-tid='SaveButton']");
        var createdFolder = driver.FindElements(By.XPath($"//div[contains(text(), '{folderName}')]"));
        
        createdFolder.Count.Should().Be(1);
    }

    [Test]
    public void Should_navigate_to_community_where_I_Member()
    {
        OpenSideMenu();
        var community = driver.FindElements(By.CssSelector("[data-tid='Community']"))
            .First(element => element.Displayed);
        community.Click();

        waiter.Until(ExpectedConditions.ElementExists(By.CssSelector("[data-tid='CommunitiesCounter']")));
        var iMemberXPath = "//*[@data-tid='PageHeader']//*[@data-tid='Tabs']//*[@data-tid='Item'][contains(text(), 'Я участник')]";
        waiter.Until(ExpectedConditions.ElementIsVisible(By.XPath(iMemberXPath)));
        var iMemberTab = driver.FindElement(By.XPath(iMemberXPath));
        iMemberTab.Click();

        waiter.Until(ExpectedConditions.UrlToBe($"{baseUrl}communities?activeTab=isMember"));
        waiter.Until(ExpectedConditions.ElementExists(By.CssSelector("[data-tid='CommunitiesCounter']")));
        
        var iMemberElements = driver.FindElement(By.CssSelector("[data-tid='Feed']"))
            .FindElements(By.XPath("//span[contains(text(), 'Я участник')]"));
        var communityCount = driver.FindElement(By.CssSelector("[data-tid='CommunitiesCounter']"))
            .FindElement(By.TagName("span")).Text.Split(" ").First();
        int.Parse(communityCount).Should().Be(iMemberElements.Count);
    }

    [Test]
    public void Should_navigate_to_employers()
    {
        OpenSideMenu();
        
        var companyButton = driver.FindElements(By.CssSelector("[data-tid='Structure']"))
            .First(element => element.Displayed);
        companyButton.Click();
        ClickOnElementByCssSelector("[data-tid='DepartmentList'] [data-tid='Item']");
        var employersTab = driver.FindElements(By.CssSelector("[data-tid='PageHeader'] [data-tid='Item']"))
            .First(element => element.Text == "Сотрудники");
        employersTab.Click();
        
        driver.Url.Should().Contain("tab=employees");
    }

    [Test]
    public void Should_comment_from_comments_page()
    {
        OpenSideMenu();
        var commentsButton = driver.FindElements(By.CssSelector("a[data-tid='Comments']"))
            .First(element => element.Displayed);
        commentsButton.Click();

        ClickOnElementByCssSelector("[data-tid='AddComment']");
        var addCommentElement = driver.FindElement(By.CssSelector("[data-tid='AddComment']"));
        addCommentElement.Click();
        var commentInputField = driver.FindElement(By.TagName("textarea"));
        var comment = GenerateGuidString();
        commentInputField.SendKeys(comment);
        ClickOnElementByCssSelector("[data-tid='SendComment']");

        var existedComment = driver.FindElements(By.XPath($"//div[contains(text(), '{comment}')]"));
        existedComment.Count.Should().Be(1);
    }

    [Test]
    public void Should_edit_address()
    {
        ClickOnElementByCssSelector("[data-tid='Avatar']");
        ClickOnElementByCssSelector("[data-tid='ProfileEdit']");
        var address = GenerateGuidString();
        var addressInputField = driver.FindElement(By.CssSelector("[data-tid='Address'] textarea"));
        addressInputField.SendKeys(address);

        var saveButton = driver.FindElements(By.CssSelector("[data-tid='PageHeader'] button"))
            .First(e => e.Text == "Сохранить");
        saveButton.Click();

        var savedAddress = driver.FindElements(By.XPath($"//div[contains(text(),'{address}')]"));
        savedAddress.Count.Should().Be(1);
    }

    private void OpenSideMenu()
    {
        ClickOnElementByCssSelector("[data-tid='SidebarMenuButton']");
    }

    private void ClickOnElementByCssSelector(string selector)
    {
        waiter.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(selector)));
        var element = driver.FindElement(By.CssSelector(selector));

        element.Click();
    }

    private void Authorization()
    {
        driver.Navigate().GoToUrl(baseUrl);
        var loginField = driver.FindElement(By.Id("Username"));
        loginField.SendKeys(login);
        var passwordField = driver.FindElement(By.Name("Password"));
        passwordField.SendKeys(password);

        ClickOnElementByCssSelector("[name='button']");

        var newsTitle = driver.FindElement(By.CssSelector("[data-tid='Title']"));
        newsTitle.Text.Should().Be("Новости");
    }

    private static string GenerateGuidString()
    {
        return Guid.NewGuid().ToString();
    }
}