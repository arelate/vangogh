package rest

import "strings"

const (
	digits           = "1234567890"
	englishLowercase = "qwertyuiopasdfghjklzxcvbnm"
	englishUppercase = "QWERTYUIOPASDFGHJKLZXCVBNM"
)

const (
	errCharactersNotAllowed = "characters not allowed"
)

func isAllowed(input string, allowed string) bool {
	for _, r := range input {
		if !strings.ContainsRune(allowed, r) {
			return false
		}
	}
	return true
}
