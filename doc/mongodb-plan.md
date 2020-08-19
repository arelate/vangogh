# Migrating from JSON to MongoDB

- [done] Download and install MongoDB
- [done] Setup auth access to localhost instance
    - [done] admin
    - [done] vangogh with readWrite access to vangogh db only
- [done] Confirm replacing data works // https://docs.mongodb.com/manual/tutorial/update-documents/
- [done] Go: connect to local DB
    - [done] using VSC connection string
- [done] Figure out a way to map _id to id
- [started] Go: migrate from JSON to MongoDB - see proposed data flow

## Proposed data flow on update operations

- Track requests responses hashes (consider sha, md5, murmur3?) for pages and details
- If response hash doesn't match stored - breakdown collection to objects and compute individual hashes
- If object hash changed:
    - Replace current object or insert new
    - Store new hash
    - Set modified (or created) to current timestamp
- Potential schemas:
    - requestHashes: {_id(url),hash,timestamp}
    - productHashes: {_id(id),hash,added,modified}
