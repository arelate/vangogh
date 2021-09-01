package hours

import "strconv"

func Atoi(str string) (int, error) {
	var sha int
	var err error
	if str != "" {
		sha, err = strconv.Atoi(str)
		if err != nil {
			return 0, err
		}
	}
	return sha, nil
}
