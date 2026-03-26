## Overview

This guide walks you through deploying **ExternalDNS with a Hydra webhook** to create your own domain record.

It supports multiple sources:

- CRDs (`DNSEndpoint`) - we use this one as we like to have a CNAME record per app in one cluster
- Services (`LoadBalancer`) 
- Contour `HTTPProxy` 

## Prerequisites

- Kubernetes cluster
- kubectl configured
- Hydra DNS API access
- Contour installed (if using HTTPProxy)
- Clone the repo

```
git clone https://github.com/mikolabarneo/HydraWebHook.git

cd HydraWebHook/HydraWebHook/K8s
```

---

## Step 1 — Install DNSEndpoint CRD

```bash
kubectl apply -f dnsendpoints.externaldns.k8s.io.yaml
```

---

## Step 2 — Create Namespace and RBAC

```bash
kubectl apply -f external-dns-init.yaml
```

---

## Step 3 — Create Hydra Secret

### Recommended (basic-auth)

Replace your token username and password in the file

```bash
nano external-dns-hydra-secret.yaml
```

Set:

```yaml
-  username: tokenUserName
-  password: tokenPassword
```

Then apply:

```bash
kubectl apply -f external-dns-hydra-secret.yaml
```

---

## Step 4 — Deploy ExternalDNS with Hydra Webhook

Configure the external DNS paramaters

```bash
nano external-dns-with-hydra-webhook.yaml
```

Set HYDRA_URI:

```yaml
env:
  - name: HYDRA_URI
    value: "https://www.networks.it.ox.ac.uk/api/ipam"
```

And Set --txt-owner-id, --source.

```yaml
spec:
  serviceAccountName: external-dns
  containers:
    - name: external-dns
      image: registry.k8s.io/external-dns/external-dns:v0.20.0
      args:
        - --log-level=debug
        - --source=crd 
        - --domain-filter=shore.ox.ac.uk
        - --provider=webhook
        - --txt-owner-id=oxfordfun # e.g. your tanzu namespace
      imagePullPolicy: Always
```

Then apply:

```bash
kubectl apply -f external-dns-with-hydra-webhook.yaml
```

With **--txt-owner-id=yourcluster**, ExternalDNS tracks ownership of DNS records using TXT records, so it only manages the records it created. This prevents conflicts between multiple clusters or systems and ensures it won’t accidentally modify or delete DNS entries owned by something else.

---

## Step 5 — Verify Deployment

```bash
kubectl get pods -n external-dns
kubectl describe pod <podname> -n external-dns
```

Expected pod running with 2 containers:

  - external-dns
  - hydra-webhook
---

## Check Logs

```bash
kubectl logs -n external-dns deploy/external-dns
```

---
