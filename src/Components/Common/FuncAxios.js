import axios from "axios";
import { ToastSuccess, ToastError, ToastWarning } from "../Common/FuncToast";

// const Host = "https://kind-northcutt.112-78-2-40.plesk.page/api/";
// const Host = "https://api.tbslogistics.com.vn/api/";
const Host = "http://localhost:8088/api/";

const getData = async (url) => {
  const get = await axios.get(Host + url);
  var data = get.data;
  return data;
};

const getDataCustom = async (url, data, header = null) => {
  let dataReturn = [];
  await axios.post(Host + url, data, header).then((response) => {
    dataReturn = response.data;
  });
  return dataReturn;
};

const getFile = async (url, fileName) => {
  axios
    .get(Host + url, {
      responseType: "arraybuffer",
    })
    .then((response) => {
      let blob = new Blob([response.data], {
          type: `${response.headers["content-type"]}`,
        }),
        url = window.URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute("download", `${fileName}`);
      document.body.appendChild(link);
      link.click();
    });
};

const postData = async (url, data, header = null) => {
  var isSuccess = 0;
  await axios
    .post(Host + url, data, header)
    .then((response) => {
      ToastSuccess(`${response.data}`);
      return (isSuccess = 1);
    })
    .catch((error) => {
      ToastError(`${error.response.data}`);
      return (isSuccess = 0);
    });

  return isSuccess;
};

const postFile = async (url, data) => {
  var isSuccess = 0;

  await axios
    .post(Host + url, data, {
      headers: { accept: "*/*", "Content-Type": "multipart/form-data" },
    })
    .then((response) => {
      ToastSuccess(`${response.data}`);
      return (isSuccess = 1);
    })
    .catch((error) => {
      ToastError(`${error.response.data}`);
      return (isSuccess = 0);
    });
  return isSuccess;
};

const putData = async (url, data, header = null) => {
  var isSuccess = 0;
  await axios
    .put(Host + url, data, header)
    .then((response) => {
      ToastSuccess(`${response.data}`);
      return (isSuccess = 1);
    })
    .catch((error) => {
      ToastError(`${error.response.data}`);
      return (isSuccess = 0);
    });

  return isSuccess;
};

const deleteData = async (url) => {
  var isSuccess = 0;
  axios.delete(Host + url).then(
    (response) => {
      ToastSuccess(`${response.data}`);
      return (isSuccess = 1);
    },
    (error) => {
      ToastSuccess(`${error.response.data}`);
      return (isSuccess = 0);
    }
  );
  return isSuccess;
};

export {
  getData,
  postData,
  putData,
  deleteData,
  postFile,
  getFile,
  getDataCustom,
};
