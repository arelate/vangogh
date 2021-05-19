package internal

import (
	"bufio"
	"os"
	"strings"
)

func ReadStdinIds() (map[string]bool, error) {
	ids := make(map[string]bool, 0)
	scanner := bufio.NewScanner(os.Stdin)
	for scanner.Scan() {
		line := scanner.Text()
		if strings.HasPrefix(line, " ") {
			continue
		}
		tokens := strings.Split(line, " ")
		if len(tokens) < 1 {
			continue
		}
		ids[tokens[0]] = true
	}
	if err := scanner.Err(); err != nil {
		return ids, err
	}
	return ids, nil
}
