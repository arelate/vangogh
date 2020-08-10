package formData

import "net/url"

func LogIn(username, password, token string) string {
	data := url.Values{
		"login[username]":   {username},
		"login[password]":   {password},
		"login[login_flow]": {"default"},
		"login[_token]":     {token},
	}
	return data.Encode()
}
