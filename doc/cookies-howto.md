# How to add cookies from the browser

Using Microsoft Edge (based on Chromium) as a reference browser for general flow. Please refer to [Cookie guides for other browsers](#cookie-guides-for-other-browsers) if you need help finding cookies in your browser.

## Copy cookie from the browser

- Open Microsoft Edge
- Navigate to edge://settings/siteData
- In the 'Search cookies' field type 'gog.com'
- Click '>' to expand entry for 'gog.com'
- Find and expand 'gog-al' entry
- Copy the value under 'Content'

## (Optional) Create cookies.json

- Check your vangogh folder
- If cookies.json doesnt' exist there - create an empty file and give it "cookies.json" name

## Add gog_al cookie

- Open cookies.json in your vangogh folder
- If your file is empty, start by adding an empty array: 
`[]`
- Add the following element to an array:
`{
    "Name": "gog-al",
    "Value": "PASTE_THE_VALUE_COPIED_FROM_THE_BROWSER_HERE",
    "Domain": "gog.com"
}`

# Cookie guides for other browsers

- Google Chrome:
- Mozilla Firefox: 
- Apple Safari:
- Opera:
