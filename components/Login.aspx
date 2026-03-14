<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs"
Inherits="KumariCinema.Login" %>

<!DOCTYPE html>
<html>
  <head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Cinema Admin - Login</title>
    <link
      href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css"
      rel="stylesheet"
    />
    <link
      href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css"
      rel="stylesheet"
    />
    <style>
      body {
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        min-height: 100vh;
        display: flex;
        align-items: center;
        justify-content: center;
        font-family:
          -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto,
          "Helvetica Neue", Arial, sans-serif;
      }

      .login-container {
        background: white;
        border-radius: 12px;
        box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
        overflow: hidden;
        max-width: 620px;
        width: 100%;
      }

      .login-header {
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: white;
        padding: 40px 20px;
        text-align: center;
      }

      .login-header h1 {
        font-size: 32px;
        font-weight: 700;
        margin: 0;
      }

      .login-header p {
        margin: 10px 0 0 0;
        opacity: 0.9;
      }

      .login-body {
        padding: 48px;
      }

      .form-group {
        margin-bottom: 20px;
      }

      .form-group label {
        font-weight: 600;
        color: #333;
        margin-bottom: 8px;
        display: block;
      }

      .form-group input {
        width: 100%;
        padding: 12px;
        border: 1px solid #ddd;
        border-radius: 6px;
        font-size: 14px;
        transition: all 0.3s ease;
      }

      .form-group input:focus {
        border-color: #667eea;
        outline: none;
        box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
      }

      .login-button {
        width: 100%;
        padding: 12px;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: white;
        border: none;
        border-radius: 6px;
        font-weight: 600;
        font-size: 16px;
        cursor: pointer;
        transition: all 0.3s ease;
        margin-top: 10px;
      }

      .login-button:hover {
        transform: translateY(-2px);
        box-shadow: 0 4px 16px rgba(102, 126, 234, 0.4);
      }

      .error-message {
        background-color: #f8d7da;
        color: #721c24;
        padding: 12px;
        border-radius: 6px;
        margin-bottom: 20px;
        font-size: 14px;
        display: none;
      }

      .error-message.show {
        display: block;
      }
    </style>
  </head>
  <body>
    <form runat="server">
      <div class="login-container">
        <div class="login-header">
          <h1><i class="fas fa-film"></i> Kumari</h1>
          <p>Cinema Admin Panel</p>
        </div>
        <div class="login-body">
          <div id="errorMessage" class="error-message">
            <asp:Label ID="errorLabel" runat="server"></asp:Label>
          </div>

          <div class="form-group">
            <label><i class="fas fa-envelope"></i> Email</label>
            <asp:TextBox
              ID="emailInput"
              runat="server"
              TextMode="Email"
              placeholder="admin@cinema.com"
            ></asp:TextBox>
            <asp:RequiredFieldValidator
              ControlToValidate="emailInput"
              ErrorMessage="Email is required"
              CssClass="text-danger"
              SetFocusOnError="true"
              Display="Dynamic"
              Font-Size="12px"
            ></asp:RequiredFieldValidator>
          </div>

          <div class="form-group">
            <label><i class="fas fa-lock"></i> Password</label>
            <asp:TextBox
              ID="passwordInput"
              runat="server"
              TextMode="Password"
              placeholder="Enter your password"
            ></asp:TextBox>
            <asp:RequiredFieldValidator
              ControlToValidate="passwordInput"
              ErrorMessage="Password is required"
              CssClass="text-danger"
              SetFocusOnError="true"
              Display="Dynamic"
              Font-Size="12px"
            ></asp:RequiredFieldValidator>
          </div>

          <asp:Button
            ID="loginButton"
            runat="server"
            Text="Login"
            CssClass="login-button"
            OnClick="Login_Click"
          />
        </div>
      </div>
    </form>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
  </body>
</html>
