// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

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
