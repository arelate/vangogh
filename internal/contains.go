package internal

func StringsContain(strs []string, str string) bool {
	for _, s := range strs {
		if str == s {
			return true
		}
	}
	return false
}
