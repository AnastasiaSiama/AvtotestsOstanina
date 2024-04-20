using System.Drawing;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumTest_Ostanina_AA;

public class SeleniumTestsForPractice
{
    private const string login = "Siama16@mail.ru";
    public const string password = "Tinctura92!";
    public ChromeDriver driver;

    [SetUp]
    public void Setup()
    {
        var options = new ChromeOptions { ImplicitWaitTimeout = 10.Seconds() };
        options.AddArguments("--no-sandbox", "--disable-extensions");

        driver = new ChromeDriver(options);
        driver.Manage().Window.Size = new Size(1200, 1200);
        Authorization();
    }

    [TearDown]
    public void TearDown()
    {
        driver.Quit();
    }

    [Test]
    public void Should_Authorize()
    {
        var news = driver.FindElement(By.CssSelector("[data-tid='Title']"));

        var currentUrl = driver.Url;
        currentUrl.Should().Be("https://staff-testing.testkontur.ru/news");
    }

    [Test]
    public void Should_navigate()
    {
        var sideMenu = driver.FindElement(By.CssSelector("[data-tid='SidebarMenuButton']"));
        sideMenu.Click();
        var community = driver.FindElements(By.CssSelector("[data-tid='Community']"))
            .First(element => element.Displayed);
        community.Click();
        var communityTitle = driver.FindElement(By.CssSelector("[data-tid='Title']"));
        Assert.That(driver.Url == "https://staff-testing.testkontur.ru/communities");
    }

    [Test]
    public void Should_create_folder()
    {
        var sideMenu = driver.FindElement(By.CssSelector("[data-tid='SidebarMenuButton']"));
        sideMenu.Click();
        var files = driver.FindElements(By.CssSelector("[data-tid='Files']"))
            .First(element => element.Displayed);
        files.Click();
        var dropdownButton = driver.FindElements(By.CssSelector("section")).First(e =>
                e.FindElements(By.CssSelector("[data-tid = 'DropdownButton']")).Count != 0)
            .FindElement(By.CssSelector("[data-tid = 'DropdownButton']"));
        dropdownButton.Click();
        var createFolder = driver.FindElement(By.CssSelector("[data-tid = 'CreateFolder']"));
        createFolder.Click();
        var input = driver.FindElement(By.CssSelector("[data-tid='Input']"));
        var folderName = GenerateString();
        input.SendKeys(folderName);
        var saveButton = driver.FindElement(By.CssSelector("[data-tid='SaveButton']"));
        saveButton.Click();
        var createdFolder = driver.FindElements(By.XPath($"//div[contains(text(), '{folderName}')]"));
        createdFolder.Count.Should().Be(1);
    }

    [Test]
    public void Should_navigate_to_community_where_I_Member()
    {
        var sideMenu = driver.FindElement(By.CssSelector("[data-tid='SidebarMenuButton']"));
        sideMenu.Click();
        var community = driver.FindElements(By.CssSelector("[data-tid='Community']"))
            .First(element => element.Displayed);
        community.Click();
        Task.Delay(2.Seconds()).GetAwaiter().GetResult();
        var item = driver.FindElement(By.CssSelector("[data-tid='PageHeader']"))
            .FindElements(By.CssSelector("[data-tid='Item']"))
            .First(element => element.Text == "Я участник");
        item.Click();
        Assert.That(driver.Url == "https://staff-testing.testkontur.ru/communities?activeTab=isMember");
    }

    [Test]
    public void Should_navigate_to_employers()
    {
        var sideMenu = driver.FindElement(By.CssSelector("[data-tid='SidebarMenuButton']"));
        sideMenu.Click();
        var companyButton = driver.FindElements(By.CssSelector("[data-tid='Structure']"))
            .First(element => element.Displayed);
        companyButton.Click();
        var company = driver.FindElement(By.CssSelector("[data-tid='DepartmentList']"))
            .FindElements(By.CssSelector("[data-tid='Item']")).First();
        company.Click();
        var item = driver.FindElement(By.CssSelector("[data-tid='PageHeader']"))
            .FindElements(By.CssSelector("[data-tid='Item']"))
            .First(element => element.Text == "Сотрудники");
        item.Click();

        Assert.That(driver.Url.Contains("tab=employees"));
    }

    [Test]
    // Зайти в раздел "Комментарии" и написать комментарий "Автотест Насти Останиной"
    public void Should_comment_from_comments_page()
    {
        var sideMenu = driver.FindElement(By.CssSelector("[data-tid='SidebarMenuButton']"));
        sideMenu.Click();
        var commentsButton = driver.FindElements(By.CssSelector("[data-tid='Comments']"))
            .First(element => element.Displayed);
        commentsButton.Click();

        var addCommentElement = driver.FindElement(By.CssSelector("[data-tid='AddComment']"));
        addCommentElement.Click();
        var commentInputField = driver.FindElement(By.TagName("textarea"));
        var comment = GenerateString();
        commentInputField.SendKeys(comment);
        var sendCommentButton = driver.FindElement(By.CssSelector("[data-tid='SendComment']"));
        sendCommentButton.Click();

        var existedComment = driver.FindElements(By.XPath($"//div[contains(text(), '{comment}')]"));
        existedComment.Count.Should().Be(1);
    }

    [Test]
    //   Зайти в раздел "Редактировать" и указать свое рабочее место, нажать на кнопку "Сохранить"
    public void Should_edit_address()
    {
        var avatarButton = driver.FindElement(By.CssSelector("[data-tid='Avatar']"));
        avatarButton.Click();
        var editButton = driver.FindElement(By.CssSelector("[data-tid='ProfileEdit']"));
        editButton.Click();
        var address = GenerateString();
        var addressInputField = driver.FindElement(By.CssSelector("[data-tid='Address']"))
            .FindElement(By.TagName("textarea"));
        addressInputField.SendKeys(address);

        var saveButton = driver.FindElement(By.CssSelector("[data-tid='PageHeader'")).FindElements(By.TagName("button"))
            .First(e => e.Text == "Сохранить");
        saveButton.Click();

        var savedAddress = driver.FindElements(By.XPath($"//div[contains(text(), '{address}')]"));
        savedAddress.Count.Should().Be(1);
    }

    private void Authorization()
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru");
        var login = driver.FindElement(By.Id("Username"));
        login.SendKeys(SeleniumTestsForPractice.login);
        var password = driver.FindElement(By.Name("Password"));
        password.SendKeys(SeleniumTestsForPractice.password);

        var enter = driver.FindElement(By.Name("button"));
        enter.Click();
    }

    private static string GenerateString()
    {
        return Guid.NewGuid().ToString();
    }
}