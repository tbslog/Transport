import { useState, useEffect } from "react";

export default function LoginPage() {
  const [UserName, SetUserName] = useState("");
  const [Password, SetPassword] = useState("");

  const handleSignInSubmit = (e) => {
    e.preventDefault();

    console.log(UserName, "----", Password);
  };

  return (
    <form onSubmit={handleSignInSubmit}>
      <div>
        <div className="sign-in-form">
          <input type="text" onChange={(e) => SetUserName(e.target.value)} />
          <input
            type="password"
            onChange={(e) => SetPassword(e.target.value)}
          />
          <button className="btn btn-sm btn-primary" type="submit">
            Login
          </button>
        </div>
      </div>
    </form>
  );
}
