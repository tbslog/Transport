import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { getData, postData, getFileImage } from "../Common/FuncAxios";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import DataTable from "react-data-table-component";
import moment from "moment";
import Lightbox from "react-awesome-lightbox";
import "react-awesome-lightbox/build/style.css";
import Cookies from "js-cookie";
import ConfirmDialog from "../Common/Dialog/ConfirmDialog";

const HandlingImage = (props) => {
  const { dataClick, checkModal } = props;
  const accountType = Cookies.get("AccType");

  const {
    register,
    reset,
    setValue,
    control,
    getValues,
    validate,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
  });

  const columns = useMemo(() => [
    {
      name: "Xem Hình",
      cell: (val) => (
        <button
          onClick={() => handleEditButtonClick(val, SetShowModal("ShowImage"))}
          type="button"
          className="btn btn-title btn-sm btn-default mx-1"
          gloss="Xem Hình Ảnh"
        >
          <i className="far fa-eye"></i>
        </button>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    {
      cell: (val) => (
        <>
          {accountType && accountType === "NV" && (
            <button
              onClick={() => showConfirmDialog(val, setFuncName("removeImage"))}
              type="button"
              className="btn btn-title btn-sm btn-default mx-1"
              gloss="Xóa Hình Ảnh"
            >
              <i className="fas fa-trash"></i>
            </button>
          )}
        </>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },

    {
      selector: (row) => row.id,
      omit: true,
    },
    {
      name: "Tên Chứng Từ",
      cell: (row) => (
        <div className="row">
          <div className="col col-9">
            <input
              autoComplete="false"
              type="text"
              className="form-control"
              id="ImageName"
              {...register(`ImageName${row.id}`)}
              onLoad={LoadNameImage(row)}
              onChange={(e) => handleOnChangeName(row.id, e.target.value)}
            />
          </div>
          <div className="col col-3">
            <button
              type="button"
              onClick={() => handleOnSave(row.id)}
              className="btn-sm mx"
            >
              <i className="fas fa-check"></i>
            </button>
          </div>
        </div>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
    },
    {
      name: "Loại File",
      selector: (row) => row.fileType,
      sortable: true,
    },
    {
      name: "Trạng Thái",
      selector: (row) => row.fileType,
      sortable: true,
    },
    {
      name: "Thời Gian Tải Lên",
      selector: (row) => moment(row.uploadedTime).format("DD/MM/YYYY HH:mm:ss"),
      sortable: true,
    },
  ]);

  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);

  const [funcName, setFuncName] = useState("");

  const [ShowModal, SetShowModal] = useState("");
  const [ShowConfirm, setShowConfirm] = useState(false);

  const [selectIdClick, setSelectIdClick] = useState({});
  const [image, setImage] = useState("");
  const [name, setName] = useState("");

  const handleDeleteImage = async () => {
    if (
      ShowConfirm === true &&
      selectIdClick &&
      Object.keys(selectIdClick).length > 0
    ) {
      const fileId = selectIdClick.id;
      var del = await postData(`BillOfLading/DeleteImage?fileId=${fileId}`);

      if (del === 1) {
        await fetchData(dataClick.maDieuPhoi);
        setShowConfirm(false);
      }
    }
  };

  const showConfirmDialog = (val) => {
    if (val) {
      setSelectIdClick(val);
    }
    setShowConfirm(true);
  };

  const handleOnChangeName = (rowId, newName) => {
    setValue(`ImageName${rowId}`, newName);
  };

  const LoadNameImage = (row) => {
    setValue(`ImageName${row.id}`, row.fileName);
  };

  const handleOnSave = async (rowId) => {
    let newName = getValues(`ImageName${rowId}`);

    if (newName && newName.length > 0) {
      const changeName = await postData(
        `BillOfLading/ChangeImageName?id=${rowId}&newName=${newName}`
      );

      if (changeName === 1) {
      }
    }
  };

  const funcAgree = async () => {
    if (funcName && funcName.length > 0) {
      switch (funcName) {
        case "removeImage":
          return await handleDeleteImage();
        case "LockDoc":
          return await ChangeStatusDoc("Lock");
        case "ConfirmDoc":
          return await ChangeStatusDoc("Confirm");
      }
    }
  };

  const ChangeStatusDoc = (action) => {
    if (action) {
      if (action === "Lock") {
      }

      if (action === "Confirm") {
      }
    }
  };

  useEffect(() => {
    if (props && dataClick && Object.keys(dataClick).length > 0) {
      fetchData(dataClick.maDieuPhoi);
    }
  }, [props, dataClick, checkModal]);

  const fetchData = async (maDieuPhoi) => {
    setLoading(true);
    const data = await getData(
      `BillOfLading/GetListImage?handlingId=${maDieuPhoi}`
    );

    setData(data);
    setLoading(false);
  };

  const handleEditButtonClick = (value) => {
    setSelectIdClick(value);
    (async () => {
      var getImage = await getFileImage(
        `BillOfLading/GetImageById?id=${value.id}`
      );
      setName(value.fileName);
      setImage(getImage);
    })();
  };

  return (
    <>
      <section className="content">
        <div className="card">
          <div className="card-header">
            <div className="container-fruid">
              <div className="row">
                <button
                  className="btn btn-title btn-sm btn-default mx-1"
                  gloss="Xác Nhận Đã Đủ Chứng Từ"
                  type="button"
                  onClick={() => showConfirmDialog(setFuncName("ConfirmDoc"))}
                >
                  <i className="far fa-check-circle"></i>
                </button>
                <button
                  className="btn btn-title btn-sm btn-default mx-1"
                  gloss="Khóa Chứng Từ"
                  type="button"
                  onClick={() => showConfirmDialog(setFuncName("LockDoc"))}
                >
                  <i className="fas fa-lock"></i>
                </button>
              </div>
            </div>
          </div>
          <div className="card-body">
            <div className="container-datatable" style={{ height: "50vm" }}>
              <DataTable
                columns={columns}
                data={data}
                progressPending={loading}
                highlightOnHover
                fixedHeader
                fixedHeaderScrollHeight="60vh"
              />
            </div>
          </div>
          <div className="card-footer"></div>
        </div>

        {ShowModal === "ShowImage" && (
          <div>
            <Lightbox
              images={[{ url: image, title: name }, {}]}
              onClose={() => SetShowModal("HideImage")}
            ></Lightbox>
          </div>
        )}
        {ShowConfirm === true && (
          <ConfirmDialog
            setShowConfirm={setShowConfirm}
            title={"Bạn có chắc chắn với quyết định này?"}
            content={
              "Khi thực hiện hành động này sẽ không thể hoàn tác lại được nữa."
            }
            funcAgree={funcAgree}
          />
        )}
      </section>
    </>
  );
};

export default HandlingImage;
