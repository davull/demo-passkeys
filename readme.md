
# Dev Setup

## Hostname

Passkeys are bound to a domain. The domain must match the domain of the certificate. To be able to use something else than `localhost`, we need to add a custom hostname (e.g. `passkeys.demo`) to the hosts file.

In windows, edit C:\Windows\System32\drivers\etc\hosts and add the following line:

```bash
127.0.0.1 passkeys.demo
```

## Certificates

To be able to use passkeys localy, you need a (self-signed) certificate which is trusted by your browser. See `ca/openssl.cnf` for configuration.

### Create Certificate Authority

```bash
cd ca/

# Generate CA key
# -out outfile        Output the key to specified file
# numbits             Size of key in bits
openssl genrsa -out private/ca.key 2048

# Generate CA cert
# -new                  New reques
# -x509                 Output an X.509 certificate structure instead of a cert request
# -key val              Key for signing, and to include unless -in given
# -days +int            Number of days cert is valid for
# -out outfile          Output file
openssl req -new -x509 `
  -key private/ca.key `
  -sha256 -days 1825 `
  -out certs/ca.crt
```

### Create Server Certificate

```bash
# Create signing request
# -newkey val           Generate new key [<alg>:]<nbits>
# -noenc                Don't encrypt private keys
# -keyout outfile       File to write private key to
# -out outfile          Output file
openssl req -new -newkey rsa:2048 `
  -keyout private/passkeys.demo.key `
  -out passkeys.demo.csr
>> Enter [passphrase]
>> Common Name: passkeys.demo

# Sign request
# -config val             A config file
# -extensions val         Extension section (override value in config file)
# -days +int              Number of days to certify the cert for
# -in infile              The input cert request(s)
# -out outfile            Where to put the output file(s)
# -cert infile            The CA cert
# -keyfile val            The CA private key
openssl ca -config openssl.cnf `
  -extensions v3_req -days 1095 `
  -in passkeys.demo.csr `
  -out certs/passkeys.demo.pem `
  -cert certs/ca.crt `
  -keyfile private/ca.key
>> [y]

# 
# Convert PEM to DER (if needed)
# 
# -in infile                 Certificate input, or CSR input file with -req (default stdin)
# -outform format            Output format (DER or PEM) - default PEM
# -out outfile               Output file - default stdout
openssl x509 `
  -in certs/passkeys.demo.pem `
  -outform der -out certs/passkeys.demo.der

# Check certificate
openssl x509 -in certs/passkeys.demo.pem -text -noout
```
