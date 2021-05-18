package internal

import (
	"bufio"
	"os"
	"strings"
)

func ReadStdinIds() ([]string, error) {
	ids := make([]string, 0)
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
		ids = append(ids, tokens[0])
	}
	if err := scanner.Err(); err != nil {
		return ids, err
	}
	return ids, nil
}
