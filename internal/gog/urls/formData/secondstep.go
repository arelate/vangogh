package formData

import "net/url"

func SecondStep(code, token string) string {
	data := url.Values{
		"second_step_authentication[token][letter_1]": {string(code[0])},
		"second_step_authentication[token][letter_2]": {string(code[1])},
		"second_step_authentication[token][letter_3]": {string(code[2])},
		"second_step_authentication[token][letter_4]": {string(code[3])},
		"second_step_authentication[_token]":          {token},
	}
	return data.Encode()
}
